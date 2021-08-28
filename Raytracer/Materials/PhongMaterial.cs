using System;
using System.Numerics;
using System.Threading;
using Raytracer.Materials.Textures;
using Raytracer.Math;
using Raytracer.Utils;

namespace Raytracer.Materials
{
	public sealed class PhongMaterial : AbstractMaterial
	{
		public ITexture Diffuse { get; set; } = new SolidColorTexture { Color = new Vector4(0.5f, 0.5f, 0.5f, 1.0f) };

		public float SpecularExponent { get; set; } = 25;

		public float Kd { get; set; } = 0.8f;

		public float Ks { get; set; } = 0.2f;

		public override Vector4 Sample(Scene scene, Ray ray, Intersection intersection, Random random, int rayDepth,
		                               Vector3 rayWeight, CastRayDelegate castRay,
		                               CancellationToken cancellationToken = default)
		{
			// Sample material
			Vector3 worldNormal = GetWorldNormal(intersection);
			Vector4 diffuse = SampleDiffuse(intersection.Uv);

			// Calculate illumination
			Vector4 illumination = GetIllumination(scene, intersection.Position, worldNormal, random);

			// Global illumination
			Vector3 giWeight = new Vector3(diffuse.X, diffuse.Y, diffuse.Z) * 2 * rayWeight;
			Vector4 globalIllumination =
				GetGlobalIllumination(scene, intersection.Position, worldNormal, random, rayDepth, giWeight, castRay, cancellationToken);

			// Combine values
			Vector4 direct = illumination / MathF.PI;
			Vector4 indirect = 2 * globalIllumination;
			Vector4 combined = direct + indirect;

			// Final diffuse color
			Vector4 finalDiffuse = ColorUtils.Multiply(combined, diffuse);

			// Specular
			Vector4 specular = GetSpecular(scene, ray.Direction, intersection.Position, worldNormal, random,
			                               SpecularExponent);

			return finalDiffuse * Kd + specular * Ks;
		}

		private Vector4 SampleDiffuse(Vector2 uv)
		{
			if (Diffuse == null)
				return Color;

			float x = uv.X / Scale.X - Offset.X;
			float y = uv.Y / Scale.Y - Offset.Y;

			Vector4 diffuse = Diffuse.Sample(x, y);
			return ColorUtils.Multiply(Color, diffuse);
		}
	}
}

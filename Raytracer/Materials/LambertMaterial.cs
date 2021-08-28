using System;
using System.Numerics;
using System.Threading;
using Raytracer.Materials.Textures;
using Raytracer.Math;
using Raytracer.Utils;

namespace Raytracer.Materials
{
	public sealed class LambertMaterial : AbstractMaterial
	{
		public ITexture Diffuse { get; set; } = new SolidColorTexture { Color = new Vector4(0.5f, 0.5f, 0.5f, 1.0f) };

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
			return ColorUtils.Multiply(combined, diffuse);
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

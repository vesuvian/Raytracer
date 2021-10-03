using System;
using System.Numerics;
using System.Threading;
using Raytracer.Materials.Textures;
using Raytracer.Math;

namespace Raytracer.Materials
{
	public sealed class LambertMaterial : AbstractMaterial
	{
		public ITexture Diffuse { get; set; } = new SolidColorTexture { Color = new Vector3(0.5f, 0.5f, 0.5f) };

		public override bool Metallic { get { return false; } }

		public override Vector3 Sample(Scene scene, Ray ray, Intersection intersection, Random random, int rayDepth,
		                               Vector3 rayWeight, CastRayDelegate castRay,
		                               CancellationToken cancellationToken = default)
		{
			// Sample material
			Vector3 worldNormal = GetWorldNormal(intersection);
			Vector3 diffuse = SampleDiffuse(intersection.Uv);

			// Calculate illumination
			Vector3 illumination = GetIllumination(scene, intersection.Position, worldNormal, random);

			// Global illumination
			Vector3 giWeight = new Vector3(diffuse.X, diffuse.Y, diffuse.Z) * 2 * rayWeight;
			Vector3 globalIllumination =
				GetGlobalIllumination(scene, intersection.Position, worldNormal, random, rayDepth, giWeight, castRay, cancellationToken);

			// Combine values
			Vector3 direct = illumination / MathF.PI;
			Vector3 indirect = 2 * globalIllumination;
			Vector3 combined = direct + indirect;

			// Final diffuse color
			return combined * diffuse;
		}

		private Vector3 SampleDiffuse(Vector2 uv)
		{
			if (Diffuse == null)
				return Color;

			float x = uv.X / Scale.X - Offset.X;
			float y = uv.Y / Scale.Y - Offset.Y;

			Vector3 diffuse = Diffuse.Sample(x, y);
			return Color * diffuse;
		}
	}
}

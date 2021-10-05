using System;
using System.Numerics;
using System.Threading;
using Raytracer.Materials.Textures;
using Raytracer.Math;

namespace Raytracer.Materials
{
	public sealed class ReflectiveMaterial : AbstractMaterial
	{
		public ITexture Roughness { get; set; } = new SolidColorTexture { Color = Vector3.Zero };

		public override bool Metallic { get { return true; } }

		public override Vector3 Sample(Scene scene, Ray ray, Intersection intersection, Random random, int rayDepth,
		                               Vector3 rayWeight, CastRayDelegate castRay,
		                               CancellationToken cancellationToken = default)
		{
			// Sample material
			Vector3 worldNormal = GetWorldNormal(intersection);
			float roughness = SampleRoughness(intersection.Uv);

			// Calculate specular
			Vector3 specular = GetSpecular(scene, ray.Direction, intersection.Position, worldNormal, random, 25);

			// Calculate reflection
			Vector3 reflection;
			if (!GetReflection(scene, ray, intersection.Position, worldNormal, roughness, random,
			                   rayDepth, rayWeight, castRay, out reflection, cancellationToken))
				reflection = Color;

			return specular * 0.2f + reflection;
		}

		private float SampleRoughness(Vector2 uv)
		{
			float x = uv.X / Scale.X - Offset.X;
			float y = uv.Y / Scale.Y - Offset.Y;

			Vector3 roughness = Roughness.Sample(x, y);
			return roughness.X;
		}
	}
}

using System;
using System.Numerics;
using Raytracer.Materials.Textures;
using Raytracer.Math;
using Raytracer.Utils;

namespace Raytracer.Materials
{
	public sealed class ReflectiveMaterial : AbstractMaterial
	{
		public ITexture Roughness { get; set; } = new SolidColorTexture { Color = ColorUtils.RgbaBlack };

		public override Vector4 Sample(Scene scene, Ray ray, Intersection intersection, Random random, int rayDepth,
		                               Vector3 rayWeight, CastRayDelegate castRay)
		{
			// Sample material
			Vector3 worldNormal = GetWorldNormal(intersection);
			float roughness = SampleRoughness(intersection.Uv);

			// Calculate specular
			Vector4 specular = GetSpecular(scene, ray.Direction, intersection.Position, worldNormal, random, 25);

			// Calculate reflection
			Vector4 reflection = GetReflection(scene, ray, intersection.Position, worldNormal, roughness, random,
			                                   rayDepth, rayWeight, castRay);

			return specular * 0.2f + reflection;
		}

		private float SampleRoughness(Vector2 uv)
		{
			float x = uv.X / Scale.X - Offset.X;
			float y = uv.Y / Scale.Y - Offset.Y;

			Vector4 roughness = Roughness.Sample(x, y);
			return roughness.X;
		}
	}
}

using System;
using System.Numerics;
using System.Threading;
using Raytracer.Materials.Textures;
using Raytracer.Math;
using Raytracer.Utils;

namespace Raytracer.Materials
{
	public sealed class RefractiveMaterial : AbstractMaterial
	{
		public float Ior { get; set; } = 1.3f;

		public ITexture Roughness { get; set; } = new SolidColorTexture { Color = ColorUtils.RgbaBlack };

		public override Vector4 Sample(Scene scene, Ray ray, Intersection intersection, Random random, int rayDepth,
		                               Vector3 rayWeight, CastRayDelegate castRay,
		                               CancellationToken cancellationToken = default)
		{
			// Sample material
			Vector3 worldNormal = GetWorldNormal(intersection);
			float roughness = SampleRoughness(intersection.Uv);
			float fresnel = Vector3Utils.Fresnel(ray.Direction, worldNormal, Ior);

			// compute refraction if it is not a case of total internal reflection
			Vector4 refractionColor =
				fresnel < 1
					? GetRefraction(scene, ray, intersection.Position, worldNormal, Ior, roughness, random, rayDepth,
					                rayWeight * (1 - fresnel), castRay, cancellationToken)
					: Vector4.Zero;

			Vector4 reflectionColor =
				fresnel > 0
					? GetReflection(scene, ray, intersection.Position, worldNormal, roughness, random, rayDepth,
					                rayWeight * fresnel, castRay, cancellationToken)
					: Vector4.Zero;

			// Calculate specular
			Vector4 specular = GetSpecular(scene, ray.Direction, intersection.Position, worldNormal, random, 25);

			// mix the two
			return reflectionColor * fresnel +
			       (1 - fresnel) * (refractionColor + specular * 0.2f);
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

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

		public float Absorption { get; set; } = 0.5f;

		public float Scatter { get; set; } = 0.5f; 

		public override bool Metallic { get { return false; } }

		public ITexture Roughness { get; set; } = new SolidColorTexture { Color = Vector3.Zero };

		public override Vector3 Sample(Scene scene, Ray ray, Intersection intersection, Random random, int rayDepth,
		                               Vector3 rayWeight, CastRayDelegate castRay,
		                               CancellationToken cancellationToken = default)
		{
			// Sample material
			Vector3 worldNormal = GetWorldNormal(intersection);
			float roughness = SampleRoughness(intersection.Uv);
			float fresnel = Vector3Utils.Fresnel(ray.Direction, worldNormal, Ior);

			// Compute refraction if it is not a case of total internal reflection
			Vector3 refractionColor =
				fresnel < 1
					? GetRefraction(scene, ray, intersection.Position, worldNormal, Ior, Scatter, roughness, random, rayDepth,
					                rayWeight * (1 - fresnel), castRay, cancellationToken)
					: Vector3.Zero;

			Vector3 reflectionColor =
				fresnel > 0
					? GetReflection(scene, ray, intersection.Position, worldNormal, roughness, random, rayDepth,
					                rayWeight * fresnel, castRay, cancellationToken)
					: Vector3.Zero;

			// Calculate specular
			Vector3 specular = GetSpecular(scene, ray.Direction, intersection.Position, worldNormal, random, 200);

			// Calculate absorption
			bool inside = intersection.FaceRatio >= 0;
			float transmittance =
				inside
					? MathUtils.Clamp(MathF.Pow(10, -Absorption * intersection.Distance), 0, 1)
					: 1;
			Vector3 tint = Vector3.Lerp(Color, Vector3.One, transmittance);

			// Mix everything
			return (reflectionColor * fresnel) +
			       (1 - fresnel) * (refractionColor * tint) +
			       (specular * 0.2f);
		}

		public override Vector3 Shadow(Ray ray, Intersection intersection, Vector3 light)
		{
			bool inside = intersection.FaceRatio >= 0;

			Vector3 worldNormal = GetWorldNormal(intersection);
			float fresnel = 1 - MathF.Abs(Vector3.Dot(worldNormal, ray.Direction));

			// Calculate absorption
			float transmittance =
				inside
					? MathUtils.Clamp(MathF.Pow(10, -Absorption * intersection.Distance), 0, 1)
					: 1;
			Vector3 tint = Vector3.Lerp(Color, Vector3.One, transmittance);

			// Calculate shadow from scatter
			float scatter = inside ? Scatter : 0;
			float distance = intersection.Distance;
			float scatterDistance = scatter == 0 ? float.MaxValue : 1 / scatter;
			float scatters = scatter == 0 ? 0 : distance / scatterDistance;
			float scatterShadow = MathF.Pow(2, -scatters);

			return light * (1 - fresnel) * tint * scatterShadow;
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

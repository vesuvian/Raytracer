using System;
using System.Numerics;
using Raytracer.Materials.Textures;
using Raytracer.Math;
using Raytracer.Utils;

namespace Raytracer.Materials
{
	public sealed class RefractiveMaterial : AbstractMaterial
	{
		public float Ior { get; set; } = 1.3f;

		public ITexture Roughness { get; set; } = new SolidColorTexture { Color = ColorUtils.RgbaBlack };

		public override Vector4 Sample(Scene scene, Ray ray, Intersection intersection, Random random, int rayDepth, CastRayDelegate castRay)
		{
			// Sample material
			Vector3 worldNormal = GetWorldNormal(intersection);
			float roughness = SampleRoughness(intersection.Uv);
			float fresnel = Fresnel(ray.Direction, worldNormal, Ior);

			// compute refraction if it is not a case of total internal reflection
			Vector4 refractionColor =
				fresnel < 1
					? GetRefraction(scene, ray, intersection.Position, worldNormal, Ior, roughness, random, rayDepth, castRay)
					: Vector4.Zero;

			Vector4 reflectionColor =
				fresnel > 0
					? GetReflection(scene, ray, intersection.Position, worldNormal, roughness, random, rayDepth, castRay)
					: Vector4.Zero;

			// mix the two
			return reflectionColor * fresnel +
			       refractionColor * (1 - fresnel);
		}

		private float SampleRoughness(Vector2 uv)
		{
			float x = uv.X / Scale.X - Offset.X;
			float y = uv.Y / Scale.Y - Offset.Y;

			Vector4 roughness = Roughness.Sample(x, y);
			return roughness.X;
		}

		private static float Fresnel(Vector3 direction, Vector3 normal, float ior) 
		{ 
			float cosi = MathUtils.Clamp(-1, 1, Vector3.Dot(direction, normal));
			float etai = 1;
			float etat = ior;

			if (cosi > 0)
			{
				float temp = etai;
				etai = etat;
				etat = temp;
			}

			// Compute sini using Snell's law
			float sint = etai / etat * MathF.Sqrt(MathF.Max(0.0f, 1 - cosi * cosi));
			if (sint >= 1)
				return 1; // Total internal reflection

			float cost = MathF.Sqrt(MathF.Max(0.0f, 1 - sint * sint));
			cosi = MathF.Abs(cosi);
			float rs = ((etat * cosi) - (etai * cost)) / ((etat * cosi) + (etai * cost));
			float rp = ((etai * cosi) - (etat * cost)) / ((etai * cosi) + (etat * cost));
			return (rs * rs + rp * rp) / 2;
		} 
	}
}

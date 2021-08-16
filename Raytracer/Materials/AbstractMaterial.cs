using System;
using System.Numerics;
using Raytracer.Extensions;
using Raytracer.Materials.Textures;
using Raytracer.Math;
using Raytracer.SceneObjects.Lights;
using Raytracer.Utils;

namespace Raytracer.Materials
{
	public abstract class AbstractMaterial : IMaterial
	{
		public Vector4 Color { get; set; } = ColorUtils.RgbaWhite;
		public ITexture Normal { get; set; } = new SolidColorTexture {Color = new Vector4(0.5f, 0.5f, 1.0f, 1.0f)};
		public float NormalScale { get; set; } = 1.0f;
		public Vector2 Scale { get; set; } = Vector2.One;
		public Vector2 Offset { get; set; }

		public abstract Vector4 Sample(Scene scene, Ray ray, Intersection intersection, Random random, int rayDepth, CastRayDelegate castRay);

		public Vector3 SampleNormal(Vector2 uv)
		{
			if (Normal == null)
				return new Vector3(0, 0, -1);

			float x = uv.X / Scale.X - Offset.X;
			float y = uv.Y / Scale.Y - Offset.Y;

			Vector4 normal = Normal.Sample(x, y);

			return Vector3.Normalize(new Vector3(normal.X - 0.5f,
												 normal.Y - 0.5f,
												 -normal.Z - 0.5f) * 2);
		}

		public Vector3 GetWorldNormal(Intersection intersection)
		{
			// Hack - flip Z and Y since normal map uses Z for "towards"
			Vector3 normalMap = SampleNormal(intersection.Uv);
			normalMap = Vector3.Normalize(new Vector3(normalMap.X * NormalScale,
													  normalMap.Z * -1,
													  normalMap.Y * NormalScale));

			// Get the normal in world space
			Matrix4x4 surface = Matrix4x4Utils.Tbn(intersection.Tangent, intersection.Bitangent, intersection.Normal);
			return surface.MultiplyNormal(normalMap);
		}

		protected static Vector4 GetIllumination(Scene scene, Vector3 position, Vector3 normal, Random random)
		{
			Vector4 sum = Vector4.Zero;

			for (int i = 0; i < scene.Lights.Count; i++)
			{
				ILight light = scene.Lights[i];
				sum += light.Sample(scene, position, normal, random);
			}

			return sum;
		}

		protected static Vector4 GetGlobalIllumination(Scene scene, Vector3 position, Vector3 normal, Random random,
		                                               int rayDepth, CastRayDelegate castRay)
		{
			Vector4 sum = Vector4.Zero;

			for (int i = 0; i < scene.GlobalIlluminationSamples; i++)
			{
				float r1 = random.NextFloat();
				float r2 = random.NextFloat();

				Vector3 randomNormal = MathUtils.UniformPointOnHemisphere(r1, r2);
				(Vector3 nt, Vector3 nb) = Vector3Utils.GetTangentAndBitangent(normal);
				Matrix4x4 surface = Matrix4x4Utils.Tbn(nt, nb, normal);
				Vector3 worldNormal = surface.MultiplyNormal(randomNormal);

				Ray giRay = new Ray
				{
					Origin = position,
					Direction = worldNormal
				};

				Vector4 sample = castRay(scene, giRay, random, rayDepth + 1);
				return r1 * sample;
			}

			return scene.GlobalIlluminationSamples == 0 ? sum : sum / scene.GlobalIlluminationSamples;
		}

		protected static Vector4 GetReflection(Scene scene, Ray ray, Vector3 position, Vector3 normal, float roughness,
		                                       Random random, int rayDepth, CastRayDelegate castRay)
		{
			float r1 = random.NextFloat();
			float r2 = random.NextFloat();

			Vector3 randomNormal = MathUtils.UniformPointOnHemisphere(r1, r2);
			randomNormal = Vector3Utils.Slerp(Vector3.UnitY, randomNormal, roughness);
			(Vector3 nt, Vector3 nb) = Vector3Utils.GetTangentAndBitangent(normal);
			Matrix4x4 surface = Matrix4x4Utils.Tbn(nt, nb, normal);
			Vector3 worldNormal = surface.MultiplyNormal(randomNormal);

			return castRay(scene, ray.Reflect(position, worldNormal), random, rayDepth + 1);
		}

		protected static Vector4 GetRefraction(Scene scene, Ray ray, Vector3 position, Vector3 normal, float ior,
		                                       float roughness,
		                                       Random random, int rayDepth, CastRayDelegate castRay)
		{
			float r1 = random.NextFloat();
			float r2 = random.NextFloat();

			Vector3 randomNormal = MathUtils.UniformPointOnHemisphere(r1, r2);
			randomNormal = Vector3Utils.Slerp(Vector3.UnitY, randomNormal, roughness);
			(Vector3 nt, Vector3 nb) = Vector3Utils.GetTangentAndBitangent(normal);
			Matrix4x4 surface = Matrix4x4Utils.Tbn(nt, nb, normal);
			Vector3 worldNormal = surface.MultiplyNormal(randomNormal);

			return castRay(scene, ray.Refract(position, worldNormal, ior), random, rayDepth + 1);
		}
	}
}

using System;
using System.Linq;
using System.Numerics;
using System.Threading;
using Raytracer.Extensions;
using Raytracer.Materials.Textures;
using Raytracer.Math;
using Raytracer.SceneObjects;
using Raytracer.SceneObjects.Lights;
using Raytracer.Utils;

namespace Raytracer.Materials
{
	public abstract class AbstractMaterial : IMaterial
	{
		public Vector3 Color { get; set; } = Vector3.One;
		public ITexture Normal { get; set; } = new SolidColorTexture {Color = new Vector3(0.5f, 0.5f, 1.0f)};
		public float NormalScale { get; set; } = 1.0f;
		public Vector2 Scale { get; set; } = Vector2.One;
		public Vector2 Offset { get; set; }
		public abstract bool Metallic { get; }

		public abstract Vector3 Sample(Scene scene, Ray ray, Intersection intersection, Random random, int rayDepth,
		                               Vector3 rayWeight, CastRayDelegate castRay,
		                               CancellationToken cancellationToken = default);

		public virtual Vector3 Shadow(Ray ray, Intersection intersection, Vector3 light)
		{
			return Vector3.Zero;
		}

		public Vector3 SampleNormal(Vector2 uv)
		{
			if (Normal == null)
				return new Vector3(0, 0, -1);

			float x = uv.X / Scale.X - Offset.X;
			float y = uv.Y / Scale.Y - Offset.Y;

			Vector3 normal = Normal.Sample(x, y);

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

		protected static Vector3 GetIllumination(Scene scene, Vector3 position, Vector3 normal, Random random)
		{
			Vector3 sum = Vector3.Zero;

			for (int i = 0; i < scene.Lights.Count; i++)
			{
				ILight light = scene.Lights[i];
				sum += light.Sample(scene, position, normal, random);
			}

			return sum;
		}

		protected Vector3 GetSpecular(Scene scene, Vector3 direction, Vector3 position, Vector3 normal,
		                              Random random, float specularExponent)
		{
			Vector3 sum = Vector3.Zero;

			for (int i = 0; i < scene.Lights.Count; i++)
			{
				ILight light = scene.Lights[i];
				Vector3 lightColor = light.Sample(scene, position, normal, random);
				Vector3 lightDir = Vector3.Normalize(light.Position - position);
				Vector3 reflectionDirection = Vector3.Reflect(-lightDir, normal);
				sum += MathF.Pow(MathF.Max(0.0f, -Vector3.Dot(reflectionDirection, direction)), specularExponent) * lightColor;
			}

			if (Metallic)
				sum *= Color;

			return sum;
		}

		protected static Vector3 GetGlobalIllumination(Scene scene, Vector3 position, Vector3 normal, Random random,
		                                               int rayDepth, Vector3 rayWeight, CastRayDelegate castRay,
		                                               CancellationToken cancellationToken = default)
		{
			Vector3 sum = Vector3.Zero;

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

				Vector3 sample;
				castRay(scene, giRay, random, rayDepth + 1,
				        rayWeight * r1 / scene.GlobalIlluminationSamples, out sample,
				        cancellationToken);

				sum += sample * r1;
			}

			return scene.GlobalIlluminationSamples == 0 ? sum : sum / scene.GlobalIlluminationSamples;
		}

		public virtual Vector3 GetAmbientOcclusion(Scene scene, Random random, Vector3 position, Vector3 normal)
		{
			Vector3 occlusionSum = Vector3.Zero;
			Vector3 occlusionMax = Vector3.One * scene.AmbientOcclusionSamples;

			for (int i = 0; i < scene.AmbientOcclusionSamples; i++)
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

				bool hit =
					scene.GetIntersections(giRay, eRayMask.AmbientOcclusion)
					     .Any(kvp => kvp.Value.RayDelta > 0.001f &&
					                 kvp.Value.RayDelta < scene.AmbientOcclusionScale);

				if (hit)
					occlusionSum += new Vector3(1) * r1;
			}

			return scene.AmbientOcclusionSamples == 0
				? Vector3.One
				: (occlusionMax - occlusionSum) / scene.AmbientOcclusionSamples;
		}

		protected bool GetReflection(Scene scene, Ray ray, Vector3 position, Vector3 normal, float roughness,
		                                Random random, int rayDepth, Vector3 rayWeight, CastRayDelegate castRay,
										out Vector3 sample, CancellationToken cancellationToken = default)
		{
			float r1 = random.NextFloat();
			float r2 = random.NextFloat();

			Vector3 randomNormal = MathUtils.UniformPointOnHemisphere(r1, r2);
			randomNormal = Vector3Utils.Slerp(Vector3.UnitY, randomNormal, roughness);
			(Vector3 nt, Vector3 nb) = Vector3Utils.GetTangentAndBitangent(normal);
			Matrix4x4 surface = Matrix4x4Utils.Tbn(nt, nb, normal);
			Vector3 worldNormal = surface.MultiplyNormal(randomNormal);

			if (!castRay(scene, ray.Reflect(position, worldNormal), random, rayDepth + 1, rayWeight, out sample,
			             cancellationToken))
				return false;

			if (Metallic)
				sample *= Color;

			return true;
		}

		protected bool GetRefraction(Scene scene, Ray ray, Vector3 position, Vector3 normal, float ior,
		                                float scatter, float roughness, Random random, int rayDepth, Vector3 rayWeight,
		                                CastRayDelegate castRay, out Vector3 sample,
		                                CancellationToken cancellationToken = default)
		{
			sample = default;

			bool inside = Vector3.Dot(ray.Direction, normal) >= 0;
			float distance = (ray.Origin - position).Length();
			float scatterDistance = scatter == 0 ? float.MaxValue : 1 / scatter;
			float scatterChance = scatter == 0 ? 0 : distance / scatterDistance;

			// Calculate scatter
			Ray refractedRay;
			if (inside && random.NextFloat() < scatterChance)
			{
				scatterDistance = random.NextFloat(0, scatterDistance);
				Vector3 scatterPosition = ray.PositionAtDelta(scatterDistance);
				Vector3 scatterDirection = MathUtils.RandomPointOnSphere(random);
				refractedRay = new Ray
				{
					Origin = scatterPosition,
					Direction = scatterDirection
				};
			}
			// Calculate refraction
			else
			{
				float r1 = random.NextFloat();
				float r2 = random.NextFloat();

				Vector3 randomNormal = MathUtils.UniformPointOnHemisphere(r1, r2);
				randomNormal = Vector3Utils.Slerp(Vector3.UnitY, randomNormal, roughness);
				(Vector3 nt, Vector3 nb) = Vector3Utils.GetTangentAndBitangent(normal);
				Matrix4x4 surface = Matrix4x4Utils.Tbn(nt, nb, normal);
				Vector3 worldNormal = surface.MultiplyNormal(randomNormal);

				if (!ray.Refract(position, worldNormal, ior, out refractedRay))
					return false;
			}

			return castRay(scene, refractedRay, random, rayDepth + 1, rayWeight, out sample, cancellationToken);
		}
	}
}

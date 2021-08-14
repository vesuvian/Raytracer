using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Raytracer.Extensions;
using Raytracer.Math;
using Raytracer.SceneObjects;
using Raytracer.SceneObjects.Geometry;
using Raytracer.Utils;

namespace Raytracer.Layers
{
	public sealed class FullLayer : AbstractLayer
	{
		protected override Vector4 CastRay(Scene scene, Ray ray, Random random, int rayDepth)
		{
			(ISceneGeometry geometry, Intersection intersection) =
				scene.GetIntersections(ray, eRayMask.Visible)
				     .Where(kvp => kvp.Value.Distance > 0.00001f)
				     .OrderBy(kvp => kvp.Value.Distance)
				     .FirstOrDefault();

			if (geometry == null)
				return ColorUtils.RgbaBlack;

			// Sample material
			Vector3 worldNormal = geometry.Material.GetWorldNormal(intersection);
			Vector4 diffuse = geometry.Material.SampleDiffuse(intersection.Uv);
			Vector4 emission = geometry.Material.SampleEmission(intersection.Uv);
			float reflectivity =
				rayDepth > scene.MaxReflectionRays
					? 0
					: geometry.Material.SampleReflectivity(intersection.Uv);
			float roughness = geometry.Material.SampleRoughness(intersection.Uv);

			// Calculate illumination
			Vector4 illumination =
				System.Math.Abs(reflectivity - 1) < 0.001f
					? ColorUtils.RgbaBlack
					: GetIllumination(scene, intersection.Position, worldNormal, random);

			// Global illumination
			Vector4 globalIllumination =
				rayDepth > scene.MaxReflectionRays ||
				System.Math.Abs(reflectivity - 1) < 0.001f
					? ColorUtils.RgbaBlack
					: GetGlobalIllumination(scene, intersection.Position, worldNormal, random, rayDepth);

			// Calculate reflection
			Vector4 reflection =
				rayDepth > scene.MaxReflectionRays ||
				System.Math.Abs(reflectivity) < 0.001f
					? ColorUtils.RgbaBlack
					: GetReflection(scene, ray, intersection.Position, worldNormal, roughness, random, rayDepth);

			// Combine values
			Vector4 direct = illumination / MathF.PI;
			Vector4 indirect = 2 * globalIllumination;
			Vector4 combined = direct + indirect;

			// Final diffuse color
			Vector4 hitColor = ColorUtils.Multiply(combined, diffuse) + emission;

			// Blend with reflection
			return ColorUtils.LerpRgb(hitColor, reflection, reflectivity);
		}

		private Vector4 GetIllumination(Scene scene, Vector3 position, Vector3 normal, Random random)
		{
			if (scene.Lights.Count == 0)
				return ColorUtils.RgbaBlack;

			IEnumerable<Vector4> illumination =
				scene.Lights
				     .Select(l => l.Sample(scene, position, normal, random));

			return ColorUtils.Sum(illumination);
		}

		private Vector4 GetGlobalIllumination(Scene scene, Vector3 position, Vector3 normal, Random random, int rayDepth)
		{
			if (scene.Geometry.Count == 0)
				return ColorUtils.RgbaBlack;

			IEnumerable<Vector4> globalIllumination =
				Enumerable.Range(0, scene.GlobalIlluminationSamples)
				          .Select(i =>
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

					          Vector4 sample = CastRay(scene, giRay, random, rayDepth + 1);
					          return r1 * sample;
				          });

			return ColorUtils.Average(globalIllumination);
		}

		private Vector4 GetReflection(Scene scene, Ray ray, Vector3 position, Vector3 normal, float roughness,
		                              Random random, int rayDepth)
		{
			float r1 = random.NextFloat();
			float r2 = random.NextFloat();

			Vector3 randomNormal = MathUtils.UniformPointOnHemisphere(r1, r2);
			randomNormal = Vector3Utils.Slerp(Vector3.UnitY, randomNormal, roughness);
			(Vector3 nt, Vector3 nb) = Vector3Utils.GetTangentAndBitangent(normal);
			Matrix4x4 surface = Matrix4x4Utils.Tbn(nt, nb, normal);
			Vector3 worldNormal = surface.MultiplyNormal(randomNormal);

			return CastRay(scene, ray.Reflect(position, worldNormal), random, rayDepth + 1);
		}
	}
}

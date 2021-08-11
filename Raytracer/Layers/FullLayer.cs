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
		protected override Vector4 CastRay(Scene scene, Ray ray, int rayDepth)
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
					: GetIllumination(scene, intersection.Position, worldNormal);

			// Global illumination
			Vector4 globalIllumination =
				rayDepth > scene.MaxReflectionRays ||
				System.Math.Abs(reflectivity - 1) < 0.001f
					? ColorUtils.RgbaBlack
					: GetGlobalIllumination(scene, intersection.Position, worldNormal, rayDepth);

			// Calculate reflection
			Vector4 reflection =
				rayDepth > scene.MaxReflectionRays ||
				System.Math.Abs(reflectivity) < 0.001f
					? ColorUtils.RgbaBlack
					: GetReflection(scene, ray, intersection.Position, worldNormal, roughness, rayDepth);

			// Combine values
			Vector4 direct = illumination / MathF.PI;
			Vector4 indirect = 2 * globalIllumination;
			Vector4 combined = direct + indirect;

			// Final diffuse color
			Vector4 hitColor = ColorUtils.Multiply(combined, diffuse) + emission;

			// Blend with reflection
			return ColorUtils.LerpRgb(hitColor, reflection, reflectivity);
		}

		private Vector4 GetIllumination(Scene scene, Vector3 position, Vector3 normal)
		{
			if (scene.Lights.Count == 0)
				return ColorUtils.RgbaBlack;

			IEnumerable<Vector4> illumination =
				scene.Lights
				     .Select(l => l.Sample(scene, position, normal));

			return ColorUtils.Sum(illumination);
		}

		private Vector4 GetGlobalIllumination(Scene scene, Vector3 position, Vector3 normal, int rayDepth)
		{
			if (scene.Geometry.Count == 0)
				return ColorUtils.RgbaBlack;

			IEnumerable<Vector4> globalIllumination =
				Enumerable.Range(0, scene.GlobalIlluminationSamples)
				          .Select(i =>
				          {
					          float r1;
					          float r2;

					          lock (Random)
					          {
						          r1 = Random.NextFloat();
						          r2 = Random.NextFloat();
					          }

					          Vector3 randomNormal = MathUtils.UniformPointOnHemisphere(r1, r2);
					          (Vector3 nt, Vector3 nb) = Vector3Utils.GetTangentAndBitangent(normal);

					          Vector3 worldNormal =
						          new Vector3(randomNormal.X * nb.X + randomNormal.Y * normal.X + randomNormal.Z * nt.X,
						                      randomNormal.X * nb.Y + randomNormal.Y * normal.Y + randomNormal.Z * nt.Y,
						                      randomNormal.X * nb.Z + randomNormal.Y * normal.Z + randomNormal.Z * nt.Z);
					          worldNormal = Vector3.Normalize(worldNormal);

					          Ray giRay = new Ray
					          {
						          Origin = position,
						          Direction = worldNormal
					          };

					          Vector4 sample = CastRay(scene, giRay, rayDepth + 1);
					          return r1 * sample;
				          });

			return ColorUtils.Average(globalIllumination);
		}

		private Vector4 GetReflection(Scene scene, Ray ray, Vector3 position, Vector3 normal, float roughness, int rayDepth)
		{
			// TODO - Random point on hemisphere and transform to surface
			float scatterAmount = new Random().NextFloat(0, roughness / 2);
			Vector3 scatterNormal;
			lock (Random)
				scatterNormal = MathUtils.RandomPointOnSphere(Random);
			Vector3 reflectionNormal = Vector3Utils.Slerp(normal, scatterNormal, scatterAmount);

			return CastRay(scene, ray.Reflect(position, reflectionNormal), rayDepth + 1);
		}
	}
}

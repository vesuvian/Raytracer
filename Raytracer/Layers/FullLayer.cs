using System;
using System.Collections.Generic;
using System.Drawing;
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
			Vector4 direct = ColorUtils.Multiply(illumination, 1 / MathF.PI);
			Vector4 indirect = ColorUtils.Multiply(globalIllumination, 2);
			Vector4 combined = ColorUtils.Add(direct, indirect);

			// Final diffuse color
			// TODO - Guessing at emission implementation
			Vector4 hitColor = Vector4.Max(ColorUtils.Multiply(combined, diffuse), emission);

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
					          // TODO - Random point on hemisphere and transform to surface
					          Vector3 randomNormal = MathUtils.RandomPointOnSphere();
					          Vector3 giNormal = Vector3Utils.Slerp(normal, randomNormal, 0.5f);

					          Ray giRay = new Ray
					          {
						          Origin = position,
						          Direction = giNormal
					          };

					          Vector4 sample = CastRay(scene, giRay, rayDepth + 1);
					          float faceAmount = Vector3.Dot(normal, giNormal);

					          return ColorUtils.LerpRgb(ColorUtils.RgbaBlack, sample, faceAmount);
				          });

			return ColorUtils.Average(globalIllumination);
		}

		private Vector4 GetReflection(Scene scene, Ray ray, Vector3 position, Vector3 normal, float roughness, int rayDepth)
		{
			// TODO - Random point on hemisphere and transform to surface
			float scatterAmount = new Random().NextFloat(0, roughness / 2);
			Vector3 scatterNormal = MathUtils.RandomPointOnSphere();
			Vector3 reflectionNormal = Vector3Utils.Slerp(normal, scatterNormal, scatterAmount);

			return CastRay(scene, ray.Reflect(position, reflectionNormal), rayDepth + 1);
		}
	}
}

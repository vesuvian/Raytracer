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
		protected override Color CastRay(Scene scene, Ray ray, int rayDepth)
		{
			if (scene.Lights.Count == 0)
				return Color.Black;

			(ISceneGeometry geometry, Intersection intersection) =
				scene.GetIntersections(ray, eRayMask.Visible)
				     .FirstOrDefault(kvp => kvp.Value.Distance > 0.00001f);

			if (geometry == null)
				return Color.Black;

			// Sample material
			Vector3 worldNormal = geometry.Material.GetWorldNormal(intersection);
			Color diffuse = geometry.Material.SampleDiffuse(intersection.Uv);
			float reflectivity =
				rayDepth > scene.MaxReflectionRays
					? 0
					: geometry.Material.SampleReflectivity(intersection.Uv);
			float roughness = geometry.Material.SampleRoughness(intersection.Uv);

			// Calculate illumination
			IEnumerable<Color> illumination =
				System.Math.Abs(reflectivity - 1) < 0.001f || scene.Lights.Count == 0
					? new[] {Color.Black}
					: scene.Lights
					       .Select(l =>
					       {
						       Color light = l.Sample(scene, intersection.Position, worldNormal);
						       return ColorUtils.Multiply(light, diffuse);
					       });
			Color illuminationSum = ColorUtils.Sum(illumination);

			// Calculate reflection
			Random random = new Random();
			float scatterAmount = random.NextFloat(0, roughness / 2);
			Vector3 scatterNormal =
				Vector3.Normalize(new Vector3(random.NextFloat(-1, 1),
				                              random.NextFloat(-1, 1),
				                              random.NextFloat(-1, 1)));
			Vector3 reflectionNormal = Vector3Utils.Slerp(worldNormal, scatterNormal, scatterAmount);

			Color reflected =
				System.Math.Abs(reflectivity) < 0.001f
					? Color.Black
					: CastRay(scene, ray.Reflect(intersection.Position, reflectionNormal), rayDepth + 1);

			// Combine values
			return ColorUtils.LerpRgb(illuminationSum, reflected, reflectivity);
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Raytracer.Math;
using Raytracer.SceneObjects;
using Raytracer.SceneObjects.Geometry;
using Raytracer.Utils;

namespace Raytracer.Layers
{
	public sealed class LightsLayer : AbstractLayer
	{
		protected override Vector4 CastRay(Scene scene, Ray ray, Random random, int rayDepth, Vector3 rayWeight, out bool hit)
		{
			hit = false;

			(ISceneGeometry geometry, Intersection intersection) =
				scene.GetIntersections(ray, eRayMask.Visible)
				     .Where(kvp => kvp.Value.RayDelta > 0.00001f)
				     .OrderBy(kvp => kvp.Value.RayDelta)
				     .FirstOrDefault();

			if (geometry == null)
				return ColorUtils.RgbaBlack;
			hit = true;

			Vector3 worldNormal = geometry.Material.GetWorldNormal(intersection);

			IEnumerable<Vector4> illumination =
				scene.Lights
				     .Select(l => l.Sample(scene, intersection.Position, worldNormal, random));

			return ColorUtils.Sum(illumination);
		}
	}
}

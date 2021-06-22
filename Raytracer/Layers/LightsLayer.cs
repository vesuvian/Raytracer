﻿using System.Drawing;
using System.Linq;
using Raytracer.Math;
using Raytracer.SceneObjects.Geometry;
using Raytracer.Utils;

namespace Raytracer.Layers
{
	public sealed class LightsLayer : AbstractLayer
	{
		protected override Color CastRay(Scene scene, Ray ray)
		{
			ISceneGeometry closest = null;
			Intersection? closestIntersection = null;

			foreach (ISceneGeometry obj in scene.Geometry)
			{
				Intersection intersection;
				if (!obj.GetIntersection(ray, out intersection))
					continue;

				if (closestIntersection != null &&
				    closestIntersection.Value.Distance <= intersection.Distance)
					continue;

				closest = obj;
				closestIntersection = intersection;
			}

			if (closest == null)
				return Color.Black;

			Color[] illumination =
				scene.Lights
				     .Where(l => l.CanSee(scene, closestIntersection.Value.Position))
				     .Select(l => l.Sample(closestIntersection.Value.Position, closestIntersection.Value.Normal))
				     .ToArray();

			return illumination.Length == 0 ? Color.Black : ColorUtils.Sum(illumination);
		}
	}
}

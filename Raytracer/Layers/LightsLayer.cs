using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
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
				     .Where(l =>
				     {
					     Ray toLight =
						     new Ray
						     {
							     Origin = closestIntersection.Value.Position,
							     Direction = Vector3.Normalize(l.Position - closestIntersection.Value.Position)
						     };

						 return
							 scene.Geometry
							       .All(g =>
							       {
								       Intersection intersection;
								       return !g.GetIntersection(toLight, out intersection) ||
								              intersection.Distance >
								              Vector3.Distance(l.Position, closestIntersection.Value.Position);
							       });
				     })
				     .Select(l =>
				     {
					     float lightAmount = Vector3.Dot(closestIntersection.Value.Normal, Vector3.Normalize(l.Position - closestIntersection.Value.Position));
					     if (lightAmount <= 0)
						     return Color.Black;

					     return Color.FromArgb((int)(lightAmount * l.Color.R),
					                           (int)(lightAmount * l.Color.G),
					                           (int)(lightAmount * l.Color.B));
				     })
				     .ToArray();

			return illumination.Length == 0 ? Color.Black : ColorUtils.Sum(illumination);
		}
	}
}

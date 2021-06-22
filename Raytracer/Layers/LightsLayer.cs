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
				     .Select(l => l.Sample(closestIntersection.Value.Position, closestIntersection.Value.Position))
				     .ToArray();

			return illumination.Length == 0 ? Color.Black : ColorUtils.Sum(illumination);
		}
	}
}

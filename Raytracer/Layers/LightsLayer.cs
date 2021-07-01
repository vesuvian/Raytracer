using System.Collections.Generic;
using System.Drawing;
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
		protected override Color CastRay(Scene scene, Ray ray)
		{
			if (scene.Lights.Count == 0)
				return Color.Black;

			(ISceneGeometry geometry, Intersection intersection) =
				scene.GetIntersections(ray, eRayMask.Visible).FirstOrDefault();

			if (geometry == null)
				return Color.Black;

			Vector3 worldNormal = geometry.GetSurfaceNormal(intersection);

			IEnumerable<Color> illumination =
				scene.Lights
				     .Select(l => l.Sample(scene, intersection.Position, worldNormal));

			return ColorUtils.Sum(illumination);
		}
	}
}

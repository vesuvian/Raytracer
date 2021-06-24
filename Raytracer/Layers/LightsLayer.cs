using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Raytracer.Math;
using Raytracer.SceneObjects;
using Raytracer.Utils;

namespace Raytracer.Layers
{
	public sealed class LightsLayer : AbstractLayer
	{
		protected override Color CastRay(Scene scene, Ray ray)
		{
			if (scene.Lights.Count == 0)
				return Color.Black;

			Intersection? closestIntersection =
				scene.GetIntersections(ray, eRayMask.Visible).Select(kvp => (Intersection?)kvp.Value).FirstOrDefault();

			if (closestIntersection == null)
				return Color.Black;

			IEnumerable<Color> illumination =
				scene.Lights
				     .Select(l => l.Sample(scene, closestIntersection.Value.Position, closestIntersection.Value.Normal));

			return ColorUtils.Sum(illumination);
		}
	}
}

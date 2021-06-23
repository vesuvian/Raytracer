using System.Drawing;
using System.Linq;
using Raytracer.Math;
using Raytracer.Utils;

namespace Raytracer.Layers
{
	public sealed class LightsLayer : AbstractLayer
	{
		protected override Color CastRay(Scene scene, Ray ray)
		{
			Intersection? closestIntersection =
				scene.GetIntersections(ray).Select(kvp => (Intersection?)kvp.Value).FirstOrDefault();

			if (closestIntersection == null)
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

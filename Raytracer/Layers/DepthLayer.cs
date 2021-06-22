using System.Drawing;
using System.Linq;
using Raytracer.Math;
using Raytracer.SceneObjects.Geometry;
using Raytracer.Utils;

namespace Raytracer.Layers
{
	public sealed class DepthLayer : AbstractLayer
	{
		protected override Color CastRay(Scene scene, Ray ray)
		{
			Intersection? closestIntersection =
				scene.GetIntersections(ray).Select(i => (Intersection?)i).FirstOrDefault();

			if (closestIntersection == null)
				return Color.Black;

			float planarDistance = Plane.Distance(scene.Camera.Position, scene.Camera.Forward, closestIntersection.Value.Position, out _);
			float t = MathUtils.Clamp(planarDistance, scene.Camera.NearPlane, scene.Camera.FarPlane) /
			          (scene.Camera.FarPlane - scene.Camera.NearPlane);
			return ColorUtils.Lerp(Color.White, Color.Black, t);
		}
	}
}

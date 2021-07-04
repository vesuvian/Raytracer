using System.Drawing;
using System.Linq;
using Raytracer.Math;
using Raytracer.SceneObjects;
using Raytracer.SceneObjects.Geometry;
using Raytracer.Utils;

namespace Raytracer.Layers
{
	public sealed class DepthLayer : AbstractLayer
	{
		protected override Color CastRay(Scene scene, Ray ray, int rayDepth)
		{
			Intersection? closestIntersection =
				scene.GetIntersections(ray, eRayMask.Visible)
				     .Select(kvp => (Intersection?)kvp.Value)
				     .OrderBy(kvp => kvp.Value.Distance)
				     .FirstOrDefault();

			if (closestIntersection == null)
				return Color.Black;

			float planarDistance = Plane.Distance(scene.Camera.Position, scene.Camera.Forward, closestIntersection.Value.Position, out _);
			float t = MathUtils.Clamp(planarDistance, scene.Camera.NearPlane, scene.Camera.FarPlane) /
			          (scene.Camera.FarPlane - scene.Camera.NearPlane);
			return ColorUtils.LerpHsl(Color.White, Color.Black, t);
		}
	}
}

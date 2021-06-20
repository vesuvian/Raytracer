using System.Drawing;
using Raytracer.Math;
using Raytracer.SceneObjects.Geometry;
using Raytracer.Utils;

namespace Raytracer.Layers
{
	public sealed class DepthLayer : AbstractLayer
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

			float planarDistance = Plane.Distance(scene.Camera.Position, scene.Camera.Forward, closestIntersection.Value.Position, out _);
			float t = MathUtils.Clamp(planarDistance, scene.Camera.NearPlane, scene.Camera.FarPlane) /
			          (scene.Camera.FarPlane - scene.Camera.NearPlane);
			return ColorUtils.Lerp(Color.White, Color.Black, t);
		}
	}
}

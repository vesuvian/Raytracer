using System.Drawing;
using System.Numerics;
using Raytracer.Math;
using Raytracer.SceneObjects.Geometry;

namespace Raytracer.Layers
{
	public sealed class WorldNormalsLayer : AbstractLayer
	{
		protected override Color CastRay(Scene scene, Ray ray)
		{
			IGeometry closest = null;
			Intersection? closestIntersection = null;

			foreach (IGeometry obj in scene.Geometry)
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

			Vector3 normalPositive = (closestIntersection.Value.Normal / 2) + (Vector3.One / 2);

			return Color.FromArgb((int)(normalPositive.X * 255),
			                      (int)(normalPositive.Y * 255),
			                      (int)(normalPositive.Z * 255));
		}
	}
}

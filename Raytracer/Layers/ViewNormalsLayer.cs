using System.Drawing;
using System.Numerics;
using Raytracer.Extensions;
using Raytracer.Math;
using Raytracer.SceneObjects.Geometry;

namespace Raytracer.Layers
{
	public sealed class ViewNormalsLayer : AbstractLayer
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

			Matrix4x4 matrix;
			Matrix4x4.Invert(scene.Camera.Projection, out matrix);

			Vector3 faceNormal = matrix.MultiplyNormal(closestIntersection.Value.Normal);
			Vector3 normalPositive = (faceNormal / 2) + (Vector3.One / 2);

			return Color.FromArgb((int)(normalPositive.X * 255),
			                      (int)(normalPositive.Y * 255),
			                      (int)(normalPositive.Z * 255));
		}
	}
}

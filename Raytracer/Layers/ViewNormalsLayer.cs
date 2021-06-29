using System.Drawing;
using System.Linq;
using System.Numerics;
using Raytracer.Extensions;
using Raytracer.Math;
using Raytracer.SceneObjects;
using Raytracer.SceneObjects.Geometry;

namespace Raytracer.Layers
{
	public sealed class ViewNormalsLayer : AbstractLayer
	{
		protected override Color CastRay(Scene scene, Ray ray)
		{
			(ISceneGeometry geometry, Intersection intersection) =
				scene.GetIntersections(ray, eRayMask.Visible).FirstOrDefault();

			if (geometry == null)
				return Color.Black;

			Vector3 normalMap = geometry.Material.SampleNormal(intersection.Uv);
			Vector3 intersectionNormal = intersection.Normal;
			// TODO - Modify normal by normal map

			Matrix4x4 matrix;
			Matrix4x4.Invert(scene.Camera.Projection, out matrix);

			Vector3 faceNormal = matrix.MultiplyNormal(intersectionNormal);
			Vector3 normalPositive = (faceNormal / 2) + (Vector3.One / 2);

			return Color.FromArgb((int)(normalPositive.X * 255),
			                      (int)(normalPositive.Y * 255),
			                      (int)(normalPositive.Z * 255));
		}
	}
}

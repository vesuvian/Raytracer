using System.Drawing;
using System.Linq;
using System.Numerics;
using Raytracer.Math;
using Raytracer.SceneObjects;

namespace Raytracer.Layers
{
	public sealed class WorldNormalsLayer : AbstractLayer
	{
		protected override Color CastRay(Scene scene, Ray ray)
		{
			Intersection? closestIntersection =
				scene.GetIntersections(ray, eRayMask.Visible).Select(kvp => (Intersection?)kvp.Value).FirstOrDefault();

			if (closestIntersection == null)
				return Color.Black;

			Vector3 normalPositive = (closestIntersection.Value.Normal / 2) + (Vector3.One / 2);

			return Color.FromArgb((int)(normalPositive.X * 255),
			                      (int)(normalPositive.Y * 255),
			                      (int)(normalPositive.Z * 255));
		}
	}
}

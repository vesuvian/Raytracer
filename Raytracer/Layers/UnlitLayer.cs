using System.Drawing;
using System.Linq;
using Raytracer.Math;
using Raytracer.SceneObjects;
using Raytracer.SceneObjects.Geometry;

namespace Raytracer.Layers
{
	public sealed class UnlitLayer : AbstractLayer
	{
		protected override Color CastRay(Scene scene, Ray ray)
		{
			(ISceneGeometry geometry, Intersection intersection) =
				scene.GetIntersections(ray, eRayMask.Visible).FirstOrDefault();

			return geometry == null ? Color.Black : geometry.Material.Sample(intersection.Uv);
		}
	}
}

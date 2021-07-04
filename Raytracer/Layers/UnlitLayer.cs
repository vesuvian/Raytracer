using System.Drawing;
using System.Linq;
using Raytracer.Math;
using Raytracer.SceneObjects;
using Raytracer.SceneObjects.Geometry;

namespace Raytracer.Layers
{
	public sealed class UnlitLayer : AbstractLayer
	{
		protected override Color CastRay(Scene scene, Ray ray, int rayDepth)
		{
			(ISceneGeometry geometry, Intersection intersection) =
				scene.GetIntersections(ray, eRayMask.Visible)
				     .OrderBy(kvp => kvp.Value.Distance)
				     .FirstOrDefault();

			return geometry == null ? Color.Black : geometry.Material.SampleDiffuse(intersection.Uv);
		}
	}
}

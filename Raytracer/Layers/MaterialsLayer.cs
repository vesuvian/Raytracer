using System;
using System.Linq;
using System.Numerics;
using Raytracer.Math;
using Raytracer.SceneObjects;
using Raytracer.SceneObjects.Geometry;
using Raytracer.Utils;

namespace Raytracer.Layers
{
	public sealed class MaterialsLayer : AbstractLayer
	{
		protected override Vector4 CastRay(Scene scene, Ray ray, Random random, int rayDepth)
		{
			if (rayDepth > scene.MaxReflectionRays)
				return Vector4.Zero;

			(ISceneGeometry geometry, Intersection intersection) =
				scene.GetIntersections(ray, eRayMask.Visible)
				     .Where(kvp => kvp.Value.RayDelta > 0.00001f)
				     .OrderBy(kvp => kvp.Value.RayDelta)
				     .FirstOrDefault();

			return geometry == null
				? ColorUtils.RgbaBlack
				: geometry.Material.Sample(scene, ray, intersection, random, rayDepth, CastRay);
		}
	}
}

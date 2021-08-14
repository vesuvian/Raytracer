using System;
using System.Linq;
using System.Numerics;
using Raytracer.Math;
using Raytracer.SceneObjects;
using Raytracer.SceneObjects.Geometry;
using Raytracer.Utils;

namespace Raytracer.Layers
{
	public sealed class WorldNormalsLayer : AbstractLayer
	{
		protected override Vector4 CastRay(Scene scene, Ray ray, Random random, int rayDepth)
		{
			(ISceneGeometry geometry, Intersection intersection) =
				scene.GetIntersections(ray, eRayMask.Visible)
				     .OrderBy(kvp => kvp.Value.Distance)
				     .FirstOrDefault();

			if (geometry == null)
				return ColorUtils.RgbaBlack;

			Vector3 worldNormal = geometry.Material.GetWorldNormal(intersection);

			Vector3 normalPositive = (worldNormal / 2) + (Vector3.One / 2);

			return new Vector4(normalPositive, 1);
		}
	}
}

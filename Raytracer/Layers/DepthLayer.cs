using System;
using System.Linq;
using System.Numerics;
using Raytracer.Math;
using Raytracer.SceneObjects;
using Raytracer.Utils;
using Plane = Raytracer.SceneObjects.Geometry.Plane;

namespace Raytracer.Layers
{
	public sealed class DepthLayer : AbstractLayer
	{
		protected override Vector4 CastRay(Scene scene, Ray ray, Random random, int rayDepth)
		{
			Intersection? closestIntersection =
				scene.GetIntersections(ray, eRayMask.Visible)
				     .Select(kvp => (Intersection?)kvp.Value)
				     .OrderBy(kvp => kvp.Value.Distance)
				     .FirstOrDefault();

			if (closestIntersection == null)
				return ColorUtils.RgbaBlack;

			float planarDistance = Plane.Distance(scene.Camera.Position, scene.Camera.Forward, closestIntersection.Value.Position, out _);
			float t = MathUtils.Clamp(planarDistance, scene.Camera.NearPlane, scene.Camera.FarPlane) /
			          (scene.Camera.FarPlane - scene.Camera.NearPlane);
			return ColorUtils.LerpHsl(new Vector4(1), ColorUtils.RgbaBlack, t);
		}
	}
}

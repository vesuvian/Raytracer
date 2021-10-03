using System;
using System.Linq;
using System.Numerics;
using System.Threading;
using Raytracer.Math;
using Raytracer.SceneObjects;
using Raytracer.Utils;
using Plane = Raytracer.SceneObjects.Geometry.Plane;

namespace Raytracer.Layers
{
	public sealed class DepthLayer : AbstractLayer
	{
		protected override Vector3 CastRay(Scene scene, Ray ray, Random random, int rayDepth, Vector3 rayWeight,
		                                   out bool hit, CancellationToken cancellationToken = default)
		{
			cancellationToken.ThrowIfCancellationRequested();

			hit = false;

			Intersection? closestIntersection =
				scene.GetIntersections(ray, eRayMask.Visible)
				     .Select(kvp => (Intersection?)kvp.Value)
				     .Where(kvp => kvp.Value.RayDelta > 0.00001f)
				     .OrderBy(kvp => kvp.Value.RayDelta)
				     .FirstOrDefault();

			if (closestIntersection == null)
				return Vector3.Zero;
			hit = true;

			float planarDistance = Plane.Distance(scene.Camera.Position, scene.Camera.Forward,
			                                      closestIntersection.Value.Position, out _);
			float t = MathUtils.Clamp(planarDistance, scene.Camera.NearPlane, scene.Camera.FarPlane) /
			          (scene.Camera.FarPlane - scene.Camera.NearPlane);
			return Vector3.Lerp(Vector3.One, Vector3.Zero, t);
		}
	}
}

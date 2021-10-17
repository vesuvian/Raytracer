using System;
using System.Linq;
using System.Numerics;
using System.Threading;
using Raytracer.Math;
using Raytracer.SceneObjects;
using Raytracer.SceneObjects.Geometry;
using Raytracer.SceneObjects.Geometry.Primitives;
using Raytracer.Utils;

namespace Raytracer.Layers
{
	public sealed class DepthLayer : AbstractLayer
	{
		protected override bool CastRay(Scene scene, Ray ray, Random random, int rayDepth, Vector3 rayWeight,
		                                   out Vector3 sample, CancellationToken cancellationToken = default)
		{
			sample = Vector3.Zero;

			cancellationToken.ThrowIfCancellationRequested();

			Intersection closestIntersection =
				scene.GetIntersections(ray, eRayMask.Visible, 0.00001f)
				     .FirstOrDefault();
            if (closestIntersection == null)
				return false;

			float planarDistance = PlaneSceneGeometry.Distance(scene.Camera.Position, scene.Camera.Forward,
			                                      closestIntersection.Position, out _);
			float t = MathUtils.Clamp(planarDistance, scene.Camera.NearPlane, scene.Camera.FarPlane) /
			          (scene.Camera.FarPlane - scene.Camera.NearPlane);
			sample = Vector3.Lerp(Vector3.One, Vector3.Zero, t);

			return true;
		}
	}
}

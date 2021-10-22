using System;
using System.Numerics;
using System.Threading;
using Raytracer.Math;
using Raytracer.SceneObjects;
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

            Intersection intersection;
            if (!scene.GetIntersection(ray, out intersection, eRayMask.Visible, 0.00001f))
	            return false;

            float planarDistance = PlaneSceneGeometry.Distance(scene.Camera.Position, scene.Camera.Forward,
                                                               intersection.Position, out _);
			float t = MathUtils.Clamp(planarDistance, scene.Camera.NearPlane, scene.Camera.FarPlane) /
			          (scene.Camera.FarPlane - scene.Camera.NearPlane);
			sample = Vector3.Lerp(Vector3.One, Vector3.Zero, t);

			return true;
		}
	}
}

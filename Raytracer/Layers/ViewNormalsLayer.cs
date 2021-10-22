using System;
using System.Numerics;
using System.Threading;
using Raytracer.Extensions;
using Raytracer.Math;
using Raytracer.SceneObjects;

namespace Raytracer.Layers
{
	public sealed class ViewNormalsLayer : AbstractLayer
	{
		protected override bool CastRay(Scene scene, Ray ray, Random random, int rayDepth, Vector3 rayWeight,
		                                   out Vector3 sample, CancellationToken cancellationToken = default)
		{
			sample = Vector3.Zero;

			cancellationToken.ThrowIfCancellationRequested();

			Intersection intersection;
			if (!scene.GetIntersection(ray, out intersection, eRayMask.Visible, 0.00001f))
				return false;

			Vector3 worldNormal = intersection.Material.GetWorldNormal(intersection);

			// Get the camera matrix
			Matrix4x4 cameraToWorld;
			Matrix4x4.Invert(scene.Camera.Projection, out cameraToWorld);

			Vector3 faceNormal = cameraToWorld.MultiplyNormal(worldNormal);
			sample = (faceNormal / 2) + (Vector3.One / 2);

			return true;
		}
	}
}

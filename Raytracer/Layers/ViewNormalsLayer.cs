﻿using System;
using System.Linq;
using System.Numerics;
using System.Threading;
using Raytracer.Extensions;
using Raytracer.Math;
using Raytracer.SceneObjects;
using Raytracer.SceneObjects.Geometry;

namespace Raytracer.Layers
{
	public sealed class ViewNormalsLayer : AbstractLayer
	{
		protected override bool CastRay(Scene scene, Ray ray, Random random, int rayDepth, Vector3 rayWeight,
		                                   out Vector3 sample, CancellationToken cancellationToken = default)
		{
			sample = Vector3.Zero;

			cancellationToken.ThrowIfCancellationRequested();

            Intersection intersection =
                scene.GetIntersections(ray, eRayMask.Visible, 0.00001f)
                     .FirstOrDefault();

            if (intersection == null)
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

﻿using System.Linq;
using System.Numerics;
using Raytracer.Extensions;
using Raytracer.Math;
using Raytracer.SceneObjects;
using Raytracer.SceneObjects.Geometry;
using Raytracer.Utils;

namespace Raytracer.Layers
{
	public sealed class ViewNormalsLayer : AbstractLayer
	{
		protected override Vector4 CastRay(Scene scene, Ray ray, int rayDepth)
		{
			(ISceneGeometry geometry, Intersection intersection) =
				scene.GetIntersections(ray, eRayMask.Visible)
				     .OrderBy(kvp => kvp.Value.Distance)
				     .FirstOrDefault();

			if (geometry == null)
				return ColorUtils.RgbaBlack;

			Vector3 worldNormal = geometry.Material.GetWorldNormal(intersection);

			// Get the camera matrix
			Matrix4x4 cameraToWorld;
			Matrix4x4.Invert(scene.Camera.Projection, out cameraToWorld);

			Vector3 faceNormal = cameraToWorld.MultiplyNormal(worldNormal);
			Vector3 normalPositive = (faceNormal / 2) + (Vector3.One / 2);

			return new Vector4(normalPositive, 1);
		}
	}
}

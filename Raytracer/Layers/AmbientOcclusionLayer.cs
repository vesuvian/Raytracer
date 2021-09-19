using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading;
using Raytracer.Math;
using Raytracer.SceneObjects;
using Raytracer.SceneObjects.Geometry;
using Raytracer.Utils;

namespace Raytracer.Layers
{
	public sealed class AmbientOcclusionLayer : AbstractLayer
	{
		protected override Vector4 CastRay(Scene scene, Ray ray, Random random, int rayDepth, Vector3 rayWeight,
		                                   out bool hit, CancellationToken cancellationToken = default)
		{
			cancellationToken.ThrowIfCancellationRequested();

			hit = false;

			(ISceneGeometry geometry, Intersection intersection) =
				scene.GetIntersections(ray, eRayMask.Visible)
				     .Where(kvp => kvp.Value.RayDelta > 0.00001f)
				     .OrderBy(kvp => kvp.Value.RayDelta)
				     .FirstOrDefault();

			if (geometry == null)
				return ColorUtils.RgbaBlack;
			hit = true;

			return
				geometry.RayMask.HasFlag(eRayMask.AmbientOcclusion)
					? geometry.Material.GetAmbientOcclusion(scene, random, intersection.Position, intersection.Normal)
					: Vector4.One;
		}
	}
}

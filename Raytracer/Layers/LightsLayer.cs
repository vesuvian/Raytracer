using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading;
using Raytracer.Extensions;
using Raytracer.Math;
using Raytracer.SceneObjects;
using Raytracer.SceneObjects.Geometry;

namespace Raytracer.Layers
{
	public sealed class LightsLayer : AbstractLayer
	{
		public LightsLayer()
		{
			Gamma = 2.2f;
		}

		protected override bool CastRay(Scene scene, Ray ray, Random random, int rayDepth, Vector3 rayWeight,
		                                   out Vector3 sample, CancellationToken cancellationToken = default)
		{
			sample = Vector3.Zero;

			cancellationToken.ThrowIfCancellationRequested();

			(ISceneGeometry geometry, Intersection intersection) =
				scene.GetIntersections(ray, eRayMask.Visible)
				     .Where(kvp => kvp.Value.RayDelta > 0.00001f)
				     .OrderBy(kvp => kvp.Value.RayDelta)
				     .FirstOrDefault();

			if (geometry == null)
				return false;

			Vector3 worldNormal = geometry.Material.GetWorldNormal(intersection);

			IEnumerable<Vector3> illumination =
				scene.Lights
				     .Select(l => l.Sample(scene, intersection.Position, worldNormal, random));

			sample = illumination.Sum();

			return true;
		}
	}
}

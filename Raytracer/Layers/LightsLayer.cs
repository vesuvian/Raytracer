using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading;
using Raytracer.Extensions;
using Raytracer.Math;
using Raytracer.SceneObjects;

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

			Intersection intersection;
			if (!scene.GetIntersection(ray, out intersection, eRayMask.Visible, 0.00001f))
				return false;

			Vector3 worldNormal = intersection.Material.GetWorldNormal(intersection);

			IEnumerable<Vector3> illumination =
				scene.Lights
				     .Select(l => l.Sample(scene, intersection.Position, worldNormal, random));

			sample = illumination.Sum();

			return true;
		}
	}
}

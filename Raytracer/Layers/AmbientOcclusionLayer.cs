using System;
using System.Linq;
using System.Numerics;
using System.Threading;
using Raytracer.Math;
using Raytracer.SceneObjects;

namespace Raytracer.Layers
{
	public sealed class AmbientOcclusionLayer : AbstractLayer
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

            sample =
                intersection.Geometry.RayMask.HasFlag(eRayMask.AmbientOcclusion)
                    ? intersection.Material.GetAmbientOcclusion(scene, random, intersection.Position,
                                                                intersection.Normal)
                    : Vector3.One;

			return true;
		}
	}
}

using System;
using System.Linq;
using System.Numerics;
using System.Threading;
using Raytracer.Extensions;
using Raytracer.Math;
using Raytracer.SceneObjects;
using Raytracer.SceneObjects.Geometry;

namespace Raytracer.Layers
{
	public abstract class AbstractMaterialsLayer : AbstractLayer
	{
		protected AbstractMaterialsLayer()
		{
			Gamma = 2.2f;
		}

		protected override bool CastRay(Scene scene, Ray ray, Random random, int rayDepth, Vector3 rayWeight,
		                                   out Vector3 sample, CancellationToken cancellationToken = default)
		{
			sample = Vector3.Zero;

			if (rayDepth > scene.MaxReflectionRays)
				return false;

			// Russian Roulette
			// Randomly terminate a path with a probability inversely equal to the throughput
			float p = MathF.Max(rayWeight.X, MathF.Max(rayWeight.Y, rayWeight.Z));
			if (random.NextFloat() / (rayDepth + 1) > p)
				return false;

			cancellationToken.ThrowIfCancellationRequested();

			// Add the energy we 'lose' by randomly terminating paths
			rayWeight *= 1 / p;

			(ISceneGeometry geometry, Intersection intersection) =
				scene.GetIntersections(ray, eRayMask.Visible)
				     .Where(kvp => kvp.Value.RayDelta > 0.00001f)
				     .OrderBy(kvp => kvp.Value.RayDelta)
				     .FirstOrDefault();

			if (geometry == null)
				return false;

			sample = geometry.Material.Sample(scene, ray, intersection, random, rayDepth, rayWeight, CastRay, cancellationToken);
			Vector3 ao = geometry.Material.GetAmbientOcclusion(scene, random, intersection.Position, intersection.Normal);
			sample *= ao;

			return true;
		}
	}
}

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
	public sealed class MaterialsLayer : AbstractLayer
	{
		public MaterialsLayer()
		{
			Gamma = 2.2f;
		}

		protected override Vector3 CastRay(Scene scene, Ray ray, Random random, int rayDepth, Vector3 rayWeight,
		                                   out bool hit, CancellationToken cancellationToken = default)
		{
			cancellationToken.ThrowIfCancellationRequested();

			hit = false;

			if (rayDepth > scene.MaxReflectionRays)
				return Vector3.Zero;

			// Russian Roulette
			// Randomly terminate a path with a probability inversely equal to the throughput
			float p = MathF.Max(rayWeight.X, MathF.Max(rayWeight.Y, rayWeight.Z));
			if (random.NextFloat() / (rayDepth + 1) > p)
				return Vector3.Zero;

			// Add the energy we 'lose' by randomly terminating paths
			rayWeight *= 1 / p;

			(ISceneGeometry geometry, Intersection intersection) =
				scene.GetIntersections(ray, eRayMask.Visible)
				     .Where(kvp => kvp.Value.RayDelta > 0.00001f)
				     .OrderBy(kvp => kvp.Value.RayDelta)
				     .FirstOrDefault();

			if (geometry == null)
				return Vector3.Zero;
			hit = true;

			Vector3 sample = geometry.Material.Sample(scene, ray, intersection, random, rayDepth, rayWeight, CastRay, cancellationToken);
			Vector3 ao = geometry.Material.GetAmbientOcclusion(scene, random, intersection.Position, intersection.Normal);
			return sample * ao;
		}
	}
}

using System;
using System.Linq;
using System.Numerics;
using System.Threading;
using Raytracer.Extensions;
using Raytracer.Math;
using Raytracer.SceneObjects;
using Raytracer.SceneObjects.Geometry;
using Raytracer.Utils;

namespace Raytracer.Layers
{
	public sealed class MaterialsLayer : AbstractLayer
	{
		public MaterialsLayer()
		{
			Gamma = 2.2f;
		}

		protected override Vector4 CastRay(Scene scene, Ray ray, Random random, int rayDepth, Vector3 rayWeight,
		                                   out bool hit, CancellationToken cancellationToken = default)
		{
			cancellationToken.ThrowIfCancellationRequested();

			hit = false;

			if (rayDepth > scene.MaxReflectionRays)
				return ColorUtils.RgbaBlack;

			// Russian Roulette
			// Randomly terminate a path with a probability inversely equal to the throughput
			float p = MathF.Max(rayWeight.X, MathF.Max(rayWeight.Y, rayWeight.Z));
			if (random.NextFloat() / (rayDepth + 1) > p)
				return ColorUtils.RgbaBlack;

			// Add the energy we 'lose' by randomly terminating paths
			rayWeight *= 1 / p;

			(ISceneGeometry geometry, Intersection intersection) =
				scene.GetIntersections(ray, eRayMask.Visible)
				     .Where(kvp => kvp.Value.RayDelta > 0.00001f)
				     .OrderBy(kvp => kvp.Value.RayDelta)
				     .FirstOrDefault();

			if (geometry == null)
				return ColorUtils.RgbaBlack;
			hit = true;

			return geometry.Material.Sample(scene, ray, intersection, random, rayDepth, rayWeight, CastRay, cancellationToken);
		}
	}
}

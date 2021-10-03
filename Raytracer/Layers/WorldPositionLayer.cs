using System;
using System.Linq;
using System.Numerics;
using System.Threading;
using Raytracer.Math;
using Raytracer.SceneObjects;

namespace Raytracer.Layers
{
	public sealed class WorldPositionLayer : AbstractLayer
	{
		public Vector3 Min { get; set; } = Vector3.One * -10;
		public Vector3 Max { get; set; } = Vector3.One * 10;

		protected override Vector3 CastRay(Scene scene, Ray ray, Random random, int rayDepth, Vector3 rayWeight,
		                                   out bool hit, CancellationToken cancellationToken = default)
		{
			cancellationToken.ThrowIfCancellationRequested();

			hit = false;

			Intersection? closestIntersection =
				scene.GetIntersections(ray, eRayMask.Visible)
				     .Where(kvp => kvp.Value.RayDelta > 0.00001f)
				     .OrderBy(kvp => kvp.Value.RayDelta)
				     .Select(kvp => (Intersection?)kvp.Value)
				     .FirstOrDefault();

			if (closestIntersection == null)
				return Vector3.Zero;
			hit = true;

			Vector3 position = Vector3.Clamp(closestIntersection.Value.Position, Min, Max) - Min;
			Vector3 range = Max - Min;

			return new Vector3(position.X / range.X,
			                   position.Y / range.Y,
			                   position.Z / range.Z);
		}
	}
}

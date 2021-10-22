using System;
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

		protected override bool CastRay(Scene scene, Ray ray, Random random, int rayDepth, Vector3 rayWeight,
		                                   out Vector3 sample, CancellationToken cancellationToken = default)
		{
			sample = Vector3.Zero;

			cancellationToken.ThrowIfCancellationRequested();

			Intersection intersection;
			if (!scene.GetIntersection(ray, out intersection, eRayMask.Visible, 0.00001f))
				return false;

			Vector3 position = Vector3.Clamp(intersection.Position, Min, Max) - Min;
			Vector3 range = Max - Min;

			sample = new Vector3(position.X / range.X,
			                     position.Y / range.Y,
			                     position.Z / range.Z);

			return true;
		}
	}
}

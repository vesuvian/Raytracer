using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Raytracer.Math;
using Raytracer.Utils;

namespace Raytracer.SceneObjects.Lights
{
	public sealed class PointLight : AbstractLight
	{
		public float Range { get; set; } = 100;
		public eFalloff Falloff { get; set; }
		public int Samples { get; set; } = 1;
		public float SoftShadowRadius { get; set; }

		public override Vector4 Sample(Scene scene, Vector3 position, Vector3 normal, Random random)
		{
			float faceAmount = Vector3.Dot(normal, Vector3.Normalize(Position - position));
			faceAmount = MathUtils.Clamp(faceAmount, 0, 1);
			float distance = Vector3.Distance(Position, position);
			Vector4 sample = Sample(distance, faceAmount);

			// Don't bother finding intersections just to draw shadow
			if (sample == ColorUtils.RgbaBlack)
				return ColorUtils.RgbaBlack;

			IEnumerable<Vector4> samples =
				GetRays(position, random)
					.Select(r =>
					{
						bool canSee =
							!CastShadows ||
							!scene.GetIntersections(r, eRayMask.CastShadows)
							      .Any(kvp => kvp.Value.Distance > SELF_SHADOW_TOLERANCE &&
							                  kvp.Value.Distance < distance);

						return canSee ? sample : ColorUtils.RgbaBlack;
					});

			return ColorUtils.Average(samples);
		}

		private Vector4 Sample(float distance, float faceAmount)
		{
			switch (Falloff)
			{
				case eFalloff.None:
					return ColorUtils.Multiply(Color, faceAmount);
				case eFalloff.Linear:
					float linearFalloff = MathF.Max(Range - distance, 0) / Range;
					return ColorUtils.Multiply(Color, faceAmount * linearFalloff);
				case eFalloff.Cubic:
					break;
				case eFalloff.Quadratic:
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}

			throw new NotImplementedException();
		}

		private IEnumerable<Ray> GetRays(Vector3 position, Random random)
		{
			return Enumerable.Range(0, Samples)
			                 .Select(i =>
			                 {
				                 Vector3 pointInSphere = MathUtils.RandomPointInSphere(random);
				                 Vector3 softShadowPosition = Position + pointInSphere * SoftShadowRadius;

				                 return new Ray
				                 {
					                 Origin = position,
					                 Direction = Vector3.Normalize(softShadowPosition - position)
				                 };
			                 });
		}
	}
}

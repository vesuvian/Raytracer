using System;
using System.Collections.Generic;
using System.Drawing;
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

		public override Color Sample(Scene scene, Vector3 position, Vector3 normal)
		{
			float faceAmount = Vector3.Dot(normal, Vector3.Normalize(Position - position));
			faceAmount = MathUtils.Clamp(faceAmount, 0, 1);
			float distance = Vector3.Distance(Position, position);
			Color sample = Sample(distance, faceAmount);

			// Don't bother finding intersections just to draw shadow
			if (sample == Color.Black)
				return Color.Black;

			IEnumerable<Color> samples =
				GetRays(position)
					.Select(r =>
					{
						bool canSee =
							!CastShadows ||
							!scene.GetIntersections(r, eRayMask.CastShadows)
							      .Any(kvp => kvp.Value.Distance > SELF_SHADOW_TOLERANCE &&
							                  kvp.Value.Distance < distance);

						return canSee ? sample : Color.Black;
					});

			return ColorUtils.Average(samples);
		}

		private Color Sample(float distance, float faceAmount)
		{
			switch (Falloff)
			{
				case eFalloff.None:
					return ColorUtils.Multiply(Color, faceAmount * Intensity);
				case eFalloff.Linear:
					float linearFalloff = MathF.Max(Range - distance, 0) / Range;
					return ColorUtils.Multiply(Color, faceAmount * Intensity * linearFalloff);
				case eFalloff.Cubic:
					break;
				case eFalloff.Quadratic:
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}

			throw new NotImplementedException();
		}

		private IEnumerable<Ray> GetRays(Vector3 position)
		{
			Random random = new Random(position.GetHashCode());

			return Enumerable.Range(0, Samples)
			                 .Select(i =>
			                 {
				                 Vector3 softShadowPosition =
					                 Position +
									 MathUtils.RandomPointInSphere(random) *
					                 SoftShadowRadius;

				                 return new Ray
				                 {
					                 Origin = position,
					                 Direction = Vector3.Normalize(softShadowPosition - position)
				                 };
			                 });
		}
	}
}

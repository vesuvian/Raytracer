using System;
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
			faceAmount = MathF.Abs(faceAmount);
			faceAmount = MathUtils.Clamp(faceAmount, 0, 1);

			float distance = Vector3.Distance(Position, position);
			Vector4 sample = Sample(distance, faceAmount);

			// Don't bother finding intersections just to draw shadow
			if (sample == ColorUtils.RgbaBlack)
				return ColorUtils.RgbaBlack;

			Vector4 sum = Vector4.Zero;

			for (int i = 0; i < Samples; i++)
			{
				Ray ray = GetRays(position, random);

				bool canSee =
					!CastShadows ||
					!scene.GetIntersections(ray, eRayMask.CastShadows)
					      .Any(kvp => kvp.Value.RayDelta > SELF_SHADOW_TOLERANCE &&
					                  kvp.Value.RayDelta < distance);

				if (canSee)
					sum += sample;
			}

			return Samples == 0 ? sum : sum / Samples;
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

		private Ray GetRays(Vector3 position, Random random)
		{
			Vector3 pointInSphere = MathUtils.RandomPointInSphere(random);
			Vector3 softShadowPosition = Position + pointInSphere * SoftShadowRadius;

			return new Ray
			{
				Origin = position,
				Direction = Vector3.Normalize(softShadowPosition - position)
			};
		}
	}
}

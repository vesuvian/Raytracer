using System;
using System.Numerics;
using Raytracer.Math;
using Raytracer.Utils;

namespace Raytracer.SceneObjects.Lights
{
	public sealed class PointLight : AbstractLight
	{
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

			Vector4 sum = Vector4.Zero;

			for (int i = 0; i < Samples; i++)
			{
				Ray ray = GetRay(position, random);
				sum += Shadow(scene, ray, distance, sample);
			}

			return Samples == 0 ? sum : sum / Samples;
		}

		private Vector4 Sample(float distance, float faceAmount)
		{
			float attenuation = GetAttenuation(distance);
			return Color * faceAmount * attenuation;
		}

		private float GetAttenuation(float distance)
		{
			return Falloff switch
			{
				eFalloff.None => 1,
				eFalloff.Linear => (1 / distance),
				eFalloff.Quadratic => (1 / (distance * distance)),
				_ => throw new ArgumentOutOfRangeException()
			};
		}

		private Ray GetRay(Vector3 position, Random random)
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

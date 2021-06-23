using System;
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

		public override Color Sample(Vector3 position, Vector3 normal)
		{
			float faceAmount = Vector3.Dot(normal, Vector3.Normalize(Position - position));
			faceAmount = MathUtils.Clamp(faceAmount, 0, 1);

			float distance = Vector3.Distance(Position, position);

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

		public override bool CanSee(Scene scene, Vector3 position)
		{
			float distanceToLight = Vector3.Distance(Position, position);

			Ray toLight =
				new Ray
				{
					Origin = position,
					Direction = Vector3.Normalize(Position - position)
				};

			return !CastShadows || !scene.GetIntersections(toLight).Any(i => i.Distance < distanceToLight);
		}
	}
}

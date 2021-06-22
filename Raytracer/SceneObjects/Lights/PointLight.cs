using System;
using System.Drawing;
using System.Numerics;
using Raytracer.Utils;

namespace Raytracer.SceneObjects.Lights
{
	public sealed class PointLight : AbstractLight
	{
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
	}
}

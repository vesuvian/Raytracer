using System;

namespace Raytracer.Utils
{
	public static class MathUtils
	{
		public const float DEG2RAD = (float)System.Math.PI / 180;

		/// <summary>
		/// Approximate floating point comparison.
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <returns></returns>
		public static bool Approximately(float a, float b)
		{
			return MathF.Abs(b - a) < MathF.Max(0.000001f * MathF.Max(MathF.Abs(a), MathF.Abs(b)), 0.000001f);
		}

		public static float Clamp(float value, float min, float max)
		{
			if (value < min)
				return min;

			if (value > max)
				return max;

			return value;
		}

		public static float Lerp(float a, float b, float t)
		{
			return a * (1 - t) + b * t;
		}
	}
}

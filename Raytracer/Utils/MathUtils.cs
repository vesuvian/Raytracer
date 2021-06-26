using System;

namespace Raytracer.Utils
{
	public static class MathUtils
	{
		public const float RAD2DEG = (float)System.Math.PI * 180;
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

		public static int Clamp(int value, int min, int max)
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

		public static float SmoothStep(float from, float to, float t)
		{
			t = Clamp(t, 0.0f, 1.0f);
			t = -2.0f * t * t * t + 3.0f * t * t;
			return to * t + from * (1.0f - t);
		}

		public static int ModPositive(int value, int mod)
		{
			return (value % mod + mod) % mod;
		}

		public static float ModPositive(float value, float mod)
		{
			return (value % mod + mod) % mod;
		}
	}
}

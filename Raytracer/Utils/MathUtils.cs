using System;
using System.Numerics;
using Raytracer.Extensions;

namespace Raytracer.Utils
{
	public static class MathUtils
	{
		public const float RAD2DEG = (float)System.Math.PI * 180;
		public const float DEG2RAD = (float)System.Math.PI / 180;
		public const float TWOPI = (float)System.Math.PI * 2;

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

		public static Vector3 RandomPointOnSphere()
		{
			return RandomPointOnSphere(new Random());
		}

		public static Vector3 RandomPointOnSphere(Random random)
		{
			Vector3 randomInSphere = RandomPointInSphere(random);
			return Vector3.Normalize(randomInSphere);
		}

		public static Vector3 RandomPointInSphere()
		{
			return RandomPointInSphere(new Random());
		}

		public static Vector3 RandomPointInSphere(Random random)
		{
			float theta = random.NextFloat(0, TWOPI);
			float v = random.NextFloat(0, 1);
			float phi = MathF.Acos(2 * v - 1);
			float r = MathF.Pow(random.NextFloat(0, 1), 1 / 3.0f);
			float x = r * MathF.Sin(phi) * MathF.Cos(theta);
			float y = r * MathF.Sin(phi) * MathF.Sin(theta);
			float z = r * MathF.Cos(phi);
			return new Vector3(x, y, z);
		}

		public static Vector3 RandomPointOnHemisphere()
		{
			return RandomPointOnHemisphere(new Random());
		}

		public static Vector3 RandomPointOnHemisphere(Random random)
		{
			Vector3 output = RandomPointOnSphere(random);
			return new Vector3(output.X, MathF.Abs(output.Y), output.Z);
		}

		public static Vector3 UniformSampleHemisphere(float r1, float r2)
		{
			float sinTheta = MathF.Sqrt(1 - r1 * r1);
			float phi = 2 * MathF.PI * r2;
			float x = sinTheta * MathF.Cos(phi);
			float z = sinTheta * MathF.Sin(phi);
			return new Vector3(x, r1, z);
		}
	}
}

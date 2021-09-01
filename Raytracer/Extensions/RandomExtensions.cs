using System;
using System.Diagnostics.CodeAnalysis;
using Raytracer.Utils;

namespace Raytracer.Extensions
{
	public static class RandomExtensions
	{
		/// <summary>
		/// Gets the next random value as a boolean.
		/// </summary>
		/// <param name="extends"></param>
		/// <returns></returns>
		public static bool NextBool([NotNull] this Random extends)
		{
			return extends.Next(0, 2) == 0;
		}

		/// <summary>
		/// Gets the next random value as a float.
		/// </summary>
		/// <param name="extends"></param>
		/// <returns></returns>
		public static float NextFloat([NotNull] this Random extends)
		{
			return (float)extends.NextDouble();
		}

		/// <summary>
		/// Gets the next random value as a float.
		/// </summary>
		/// <param name="extends"></param>
		/// <param name="maxValue"></param>
		/// <returns></returns>
		public static float NextFloat([NotNull] this Random extends, float maxValue)
		{
			return extends.NextFloat(0, maxValue);
		}

		/// <summary>
		/// Gets the next random value as a float.
		/// </summary>
		/// <param name="extends"></param>
		/// <param name="minValue"></param>
		/// <param name="maxValue"></param>
		/// <returns></returns>
		public static float NextFloat([NotNull] this Random extends, float minValue, float maxValue)
		{
			float random = extends.NextFloat();
			return MathUtils.Lerp(minValue, maxValue, random);
		}
	}
}

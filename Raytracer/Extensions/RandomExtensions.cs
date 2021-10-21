using System;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using Raytracer.Utils;

namespace Raytracer.Extensions
{
	public static class RandomExtensions
	{
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

		/// <summary>
		/// Gets the next random value as a Vector3.
		/// </summary>
		/// <param name="extends"></param>
		/// <returns></returns>
		public static Vector3 NextVector3(this Random extends)
		{
			return extends.NextVector3(Vector3.One);
		}

		/// <summary>
		/// Gets the next random value as a Vector3.
		/// </summary>
		/// <param name="extends"></param>
		/// <param name="maxValue"></param>
		/// <returns></returns>
		public static Vector3 NextVector3(this Random extends, Vector3 maxValue)
		{
			return extends.NextVector3(Vector3.Zero, maxValue);
		}

		/// <summary>
		/// Gets the next random value as a Vector3.
		/// </summary>
		/// <param name="extends"></param>
		/// <param name="minValue"></param>
		/// <param name="maxValue"></param>
		/// <returns></returns>
		public static Vector3 NextVector3(this Random extends, Vector3 minValue, Vector3 maxValue)
		{
			return new Vector3(extends.NextFloat(minValue.X, maxValue.X),
			                   extends.NextFloat(minValue.Y, maxValue.Y),
			                   extends.NextFloat(minValue.Z, maxValue.Z));
		}
	}
}

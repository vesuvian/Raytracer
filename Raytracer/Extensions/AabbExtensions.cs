using System;
using System.Collections.Generic;
using Raytracer.Geometry;

namespace Raytracer.Extensions
{
	public static class AabbExtensions
	{
		public static Aabb Sum(this IEnumerable<Aabb> extends)
		{
			Aabb? output = null;

			foreach (Aabb item in extends)
				output = output == null ? item : output.Value + item;

			if (output == null)
				throw new InvalidOperationException();

			return output.Value;
		}
	}
}

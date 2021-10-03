using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace Raytracer.Extensions
{
	public static class Vector4Extensions
	{
		public static Vector3 ToVector3(this Vector4 extends)
		{
			return new Vector3(extends.X, extends.Y, extends.Z);
		}

		public static Vector4 Sum(this IEnumerable<Vector4> extends)
		{
			return extends.Aggregate((a, b) => a + b);
		}
	}
}

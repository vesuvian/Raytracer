using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace Raytracer.Extensions
{
	public static class Vector3Extensions
	{
		public static Vector4 ToVector4(this Vector3 extends, float w = 0)
		{
			return new Vector4(extends.X, extends.Y, extends.Z, w);
		}

		public static Vector3 Sum(this IEnumerable<Vector3> extends)
		{
			return extends.Aggregate((a, b) => a + b);
		}
	}
}

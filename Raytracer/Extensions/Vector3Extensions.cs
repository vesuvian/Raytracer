using System.Numerics;

namespace Raytracer.Extensions
{
	public static class Vector3Extensions
	{
		public static Vector4 ToVector4(this Vector3 extends, float w = 0)
		{
			return new Vector4(extends.X, extends.Y, extends.Z, w);
		}
	}
}

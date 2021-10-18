using System.Numerics;

namespace Raytracer.Extensions
{
	public static class Vector2Extensions
	{
		public static Vector3 ToVector3(this Vector2 extends, float z = 0)
		{
			return new Vector3(extends.X, extends.Y, z);
		}
	}
}

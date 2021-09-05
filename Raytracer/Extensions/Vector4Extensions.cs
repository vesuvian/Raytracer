using System.Numerics;

namespace Raytracer.Extensions
{
	public static class Vector4Extensions
	{
		public static Vector3 ToVector3(this Vector4 extends)
		{
			return new Vector3(extends.X, extends.Y, extends.Z);
		}
	}
}

using System.Numerics;

namespace Raytracer.Utils
{
	public static class Matrix4x4Utils
	{
		public static Matrix4x4 Trs(Vector3 translation, Quaternion rotation, Vector3 scale)
		{
			return Matrix4x4.CreateScale(scale) *
			       Matrix4x4.CreateFromQuaternion(rotation) *
			       Matrix4x4.CreateTranslation(translation);
		}

		public static Matrix4x4 Tbn(Vector3 tangent, Vector3 bitangent, Vector3 normal)
		{
			Matrix4x4 output = Matrix4x4.Identity;

			output.M11 = tangent.X;
			output.M12 = tangent.Y;
			output.M13 = tangent.Z;

			output.M21 = normal.X;
			output.M22 = normal.Y;
			output.M23 = normal.Z;

			output.M31 = bitangent.X;
			output.M32 = bitangent.Y;
			output.M33 = bitangent.Z;

			return output;
		}
	}
}

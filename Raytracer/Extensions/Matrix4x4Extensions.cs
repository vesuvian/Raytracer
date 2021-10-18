using System.Numerics;
using System.Text;

namespace Raytracer.Extensions
{
// ReSharper disable InconsistentNaming
	public static class Matrix4x4Extensions 
// ReSharper restore InconsistentNaming
	{
		public static Vector3 MultiplyNormal(this Matrix4x4 matrix, Vector3 vector)
		{
			Matrix4x4.Invert(Matrix4x4.Transpose(matrix), out matrix);
			Vector3 direction = MultiplyDirection(matrix, vector);
			return Vector3.Normalize(direction);
		}

		public static Vector3 MultiplyDirection(this Matrix4x4 matrix, Vector3 vector)
		{
			Vector3 res;
			res.X = matrix.M11 * vector.X + matrix.M21 * vector.Y + matrix.M31 * vector.Z;
			res.Y = matrix.M12 * vector.X + matrix.M22 * vector.Y + matrix.M32 * vector.Z;
			res.Z = matrix.M13 * vector.X + matrix.M23 * vector.Y + matrix.M33 * vector.Z;
			return res;
		}

		public static Vector3 MultiplyPoint(this Matrix4x4 matrix, Vector3 point)
		{
			Vector3 res;

			res.X = matrix.M11 * point.X + matrix.M21 * point.Y + matrix.M31 * point.Z + matrix.M41;
			res.Y = matrix.M12 * point.X + matrix.M22 * point.Y + matrix.M32 * point.Z + matrix.M42;
			res.Z = matrix.M13 * point.X + matrix.M23 * point.Y + matrix.M33 * point.Z + matrix.M43;
			var w = matrix.M14 * point.X + matrix.M24 * point.Y + matrix.M34 * point.Z + matrix.M44;

			w = 1F / w;
			res.X *= w;
			res.Y *= w;
			res.Z *= w;

			return res;
		}

		public static Vector4 MultiplyPoint(this Matrix4x4 matrix, Vector4 point)
		{
			Vector4 res;
			res.X = matrix.M11 * point.X + matrix.M21 * point.Y + matrix.M31 * point.Z + matrix.M41 * point.W;
			res.Y = matrix.M12 * point.X + matrix.M22 * point.Y + matrix.M32 * point.Z + matrix.M42 * point.W;
			res.Z = matrix.M13 * point.X + matrix.M23 * point.Y + matrix.M33 * point.Z + matrix.M43 * point.W;
			res.W = matrix.M14 * point.X + matrix.M24 * point.Y + matrix.M34 * point.Z + matrix.M44 * point.W;
			return res;
		}

		public static string ToStringTable(this Matrix4x4 matrix)
		{
			return new StringBuilder().AppendLine($"{matrix.M11}, {matrix.M12}, {matrix.M13}, {matrix.M14},")
			                          .AppendLine($"{matrix.M21}, {matrix.M22}, {matrix.M23}, {matrix.M24},")
			                          .AppendLine($"{matrix.M31}, {matrix.M32}, {matrix.M33}, {matrix.M34},")
			                          .AppendLine($"{matrix.M41}, {matrix.M42}, {matrix.M43}, {matrix.M44},")
			                          .ToString();
		}
	}
}

using System;
using System.Numerics;
using System.Text;
using Raytracer.Extensions;

namespace Raytracer.Math
{
	public struct Intersection : IEquatable<Intersection>
	{
		#region Properties

		public Vector3 Position { get; set; }
		public Vector3 Normal { get; set; }
		public Vector3 RayOrigin { get; set; }
		public Vector2 Uv { get; set; }

		public float Distance
		{
			get { return Vector3.Distance(Position, RayOrigin); }
		}

		#endregion

		#region Methods

		public Intersection Multiply(Matrix4x4 matrix)
		{
			return new Intersection
			{
				Position = matrix.MultiplyPoint(Position),
				Normal = matrix.MultiplyNormal(Normal),
				RayOrigin = matrix.MultiplyPoint(RayOrigin),
				Uv = Uv
			};
		}

		public override string ToString()
		{
			StringBuilder builder = new StringBuilder("\r\n");
			builder.AppendLine(string.Format("Position: {0}", Position.ToString()));
			builder.AppendLine(string.Format("Normal: {0}", Normal.ToString()));
			builder.AppendLine(string.Format("RayOrigin: {0}", RayOrigin.ToString()));
			builder.AppendLine(string.Format("Distance: {0}", Distance));

			return builder.ToString();
		}

		#endregion

		#region Equality

		public static bool operator ==(Intersection i1, Intersection i2)
		{
			return i1.Equals(i2);
		}

		public static bool operator !=(Intersection i1, Intersection i2)
		{
			return !i1.Equals(i2);
		}

		public bool Equals(Intersection other)
		{
			return Position.Equals(other.Position) &&
			       Normal.Equals(other.Normal) &&
			       RayOrigin.Equals(other.RayOrigin) &&
			       Uv.Equals(other.Uv);
		}

		public override bool Equals(object obj)
		{
			return obj is Intersection other && Equals(other);
		}

		public override int GetHashCode()
		{
			return HashCode.Combine(Position, Normal, RayOrigin, Uv);
		}

		#endregion
	}
}

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
		public Vector3 Tangent { get; set; }
		public Vector3 Bitangent { get; set; }
		public Ray Ray { get; set; }
		public Vector2 Uv { get; set; }

		public float FaceRatio { get { return Vector3.Dot(Ray.Direction, Normal); } }

		public float Distance { get { return (Position - Ray.Origin).Length(); } }

		public float RayDelta
		{
			get
			{
				bool inFront = Vector3.Dot(Vector3.Normalize(Position - Ray.Origin), Ray.Direction) > 0;
				return inFront ? Distance : -Distance;
			}
		}

		#endregion

		#region Methods

		public Intersection Multiply(Matrix4x4 matrix)
		{
			return new Intersection
			{
				Position = matrix.MultiplyPoint(Position),
				Normal = matrix.MultiplyNormal(Normal),
				Tangent = Vector3.Normalize(matrix.MultiplyDirection(Tangent)),
				Bitangent = Vector3.Normalize(matrix.MultiplyDirection(Bitangent)),
				Ray = Ray.Multiply(matrix),
				Uv = Uv
			};
		}

		public Intersection Flip()
		{
			return new Intersection
			{
				Position = Position,
				Normal = Normal * -1,
				Tangent = Tangent * -1,
				Bitangent = Bitangent,
				Ray = Ray,
				Uv = Uv
			};
		}

		public override string ToString()
		{
			StringBuilder builder = new StringBuilder("\r\n");
			builder.AppendLine(string.Format("Position: {0}", Position.ToString()));
			builder.AppendLine(string.Format("Normal: {0}", Normal.ToString()));
			builder.AppendLine(string.Format("Tangent: {0}", Tangent.ToString()));
			builder.AppendLine(string.Format("Bitangent: {0}", Bitangent.ToString()));
			builder.AppendLine(string.Format("Ray: {0}", Ray.ToString()));
			builder.AppendLine(string.Format("Uv: {0}", Uv));

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
			       Tangent.Equals(other.Tangent) &&
			       Bitangent.Equals(other.Bitangent) &&
			       Ray.Equals(other.Ray) &&
			       RayDelta.Equals(other.RayDelta) &&
			       Uv.Equals(other.Uv);
		}

		public override bool Equals(object obj)
		{
			return obj is Intersection other && Equals(other);
		}

		public override int GetHashCode()
		{
			return HashCode.Combine(Position, Normal, Tangent, Bitangent, Ray, RayDelta, Uv);
		}

		#endregion
	}
}

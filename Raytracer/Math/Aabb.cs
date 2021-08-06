using System.Collections.Generic;
using System.Numerics;
using Raytracer.Extensions;

namespace Raytracer.Math
{
	public struct Aabb
	{
		private Vector3 m_Min;
		private Vector3 m_Max;
		private Vector3[] m_Bounds;

		public Vector3 Min
		{
			get { return m_Min; }
			set
			{
				m_Min = value;
				m_Bounds = new[] {m_Min, m_Max};
			}
		}

		public Vector3 Max
		{
			get { return m_Max; }
			set
			{
				m_Max = value;
				m_Bounds = new[] {m_Min, m_Max};
			}
		}

		public Vector3[] Bounds { get { return m_Bounds ?? new Vector3[2]; } }

		public bool Intersects(Ray ray)
		{
			Vector3[] bounds = Bounds;

			float tmin = (bounds[ray.Sign.X].X - ray.Origin.X) * ray.InverseDirection.X;
			float tmax = (bounds[1 - ray.Sign.X].X - ray.Origin.X) * ray.InverseDirection.X;
			float tymin = (bounds[ray.Sign.Y].Y - ray.Origin.Y) * ray.InverseDirection.Y;
			float tymax = (bounds[1 - ray.Sign.Y].Y - ray.Origin.Y) * ray.InverseDirection.Y;

			if (tmin > tymax || tymin > tmax)
				return false;

			if (tymin > tmin)
				tmin = tymin;
			if (tymax < tmax)
				tmax = tymax;

			float tzmin = (bounds[ray.Sign.Z].Z - ray.Origin.Z) * ray.InverseDirection.Z;
			float tzmax = (bounds[1 - ray.Sign.Z].Z - ray.Origin.Z) * ray.InverseDirection.Z;

			if (tmin > tzmax || tzmin > tmax)
				return false;

			if (tzmin > tmin)
				tmin = tzmin;
			if (tzmax < tmax)
				tmax = tzmax;

			if (tmin >= 0)
				return true;

			return tmax >= 0;
		}

		public Aabb Multiply(Matrix4x4 transform)
		{
			return FromPoints(transform,
			                  new Vector3(Min.X, Min.Y, Min.Z),
			                  new Vector3(Min.X, Max.Y, Min.Z),
			                  new Vector3(Max.X, Min.Y, Min.Z),
			                  new Vector3(Max.X, Max.Y, Min.Z),
			                  new Vector3(Min.X, Min.Y, Max.Z),
			                  new Vector3(Min.X, Max.Y, Max.Z),
			                  new Vector3(Max.X, Min.Y, Max.Z),
			                  new Vector3(Max.X, Max.Y, Max.Z));
		}

		public static Aabb FromPoints(Matrix4x4 transform, params Vector3[] points)
		{
			return FromPoints(transform, (IEnumerable<Vector3>)points);
		}

		public static Aabb FromPoints(Matrix4x4 transform, IEnumerable<Vector3> points)
		{
			float minX = float.MaxValue;
			float minY = float.MaxValue;
			float minZ = float.MaxValue;
			float maxX = float.MinValue;
			float maxY = float.MinValue;
			float maxZ = float.MinValue;

			foreach (Vector3 point in points)
			{
				Vector3 transformed = transform.MultiplyPoint(point);

				if (transformed.X < minX)
					minX = transformed.X;

				if (transformed.Y < minY)
					minY = transformed.Y;

				if (transformed.Z < minZ)
					minZ = transformed.Z;

				if (transformed.X > maxX)
					maxX = transformed.X;

				if (transformed.Y > maxY)
					maxY = transformed.Y;

				if (transformed.Z > maxZ)
					maxZ = transformed.Z;
			}

			return new Aabb
			{
				Min = new Vector3(minX, minY, minZ),
				Max = new Vector3(maxX, maxY, maxZ)
			};
		}
	}
}

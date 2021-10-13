﻿using System;
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

		public Vector3 Center { get { return (Min + Max) / 2; } }

		public Vector3[] Bounds { get { return m_Bounds ?? new Vector3[2]; } }

        public bool IsInfinite
        {
            get
            {
                return float.IsInfinity(Min.X) ||
                       float.IsInfinity(Min.Y) ||
                       float.IsInfinity(Min.Z) ||
                       float.IsInfinity(Max.X) ||
                       float.IsInfinity(Max.Y) ||
                       float.IsInfinity(Max.Z);
            }
        }

        public Vector3 Extents { get { return (Max - Min) / 2; } }

        public static Aabb operator +(Aabb a, Aabb b)
		{
            return new Aabb
            {
                Min = Vector3.Min(a.Min, b.Min),
                Max = Vector3.Max(a.Max, b.Max)
            };
		}

		public static Aabb operator -(Aabb a, Aabb b)
		{
			// Slice along X
            if (b.Min.Y <= a.Min.Y &&
                b.Max.Y >= a.Max.Y &&
                b.Min.Z <= a.Min.Z &&
                b.Max.Z >= a.Max.Z)
            {
				// Trim left
                if (b.Min.X <= a.Min.X && b.Max.X >= a.Min.X)
                    a = new Aabb
                    {
                        Min = new Vector3(MathF.Min(a.Max.X, b.Max.X), a.Min.Y, a.Min.Z),
                        Max = a.Max
                    };

                // Trim right
                if (b.Max.X >= a.Max.X && b.Min.X <= a.Max.X)
                    a = new Aabb
                    {
                        Min = a.Min,
                        Max = new Vector3(MathF.Max(a.Min.X, b.Min.X), a.Max.Y, a.Max.Z)
					};
            }

            // Slice along Y
            if (b.Min.X <= a.Min.X &&
                b.Max.X >= a.Max.X &&
                b.Min.Z <= a.Min.Z &&
                b.Max.Z >= a.Max.Z)
            {
                // Trim bottom
                if (b.Min.Y <= a.Min.Y && b.Max.Y >= a.Min.Y)
                    a = new Aabb
                    {
                        Min = new Vector3(a.Min.X, MathF.Min(a.Max.Y, b.Max.Y), a.Min.Z),
                        Max = a.Max
                    };

                // Trim right
                if (b.Max.X >= a.Max.X && b.Min.X <= a.Max.X)
                    a = new Aabb
                    {
                        Min = a.Min,
                        Max = new Vector3(a.Max.X, MathF.Max(a.Min.Y, b.Min.Y), a.Max.Z)
                    };
			}

            // Slice along Z
            if (b.Min.Y <= a.Min.Y &&
                b.Max.Y >= a.Max.Y &&
                b.Min.X <= a.Min.X &&
                b.Max.X >= a.Max.X)
            {
                // Trim back
                if (b.Min.Z <= a.Min.Z && b.Max.Z >= a.Min.Z)
                    a = new Aabb
                    {
                        Min = new Vector3(a.Min.X, a.Min.Y, MathF.Min(a.Max.Z, b.Max.Z)),
                        Max = a.Max
                    };

                // Trim front
                if (b.Max.Z >= a.Max.Z && b.Min.Z <= a.Max.Z)
                    a = new Aabb
                    {
                        Min = a.Min,
                        Max = new Vector3(a.Max.X, a.Max.Y, MathF.Max(a.Min.Z, b.Min.Z))
                    };
            }

            return a;
        }

		public Aabb Intersection(Aabb other)
        {
            return new Aabb
            {
                Min = Vector3.Max(Min, other.Min),
                Max = Vector3.Min(Max, other.Max)
            };
        }

        /// <summary>
        /// Returns true if the given AABB is entirely contained in this AABB.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Contains(Aabb other)
        {
            return Contains(other.Min) && Contains(other.Max);
        }

        public bool Contains(Vector3 position)
        {
            return position.X >= Min.X &&
                   position.Y >= Min.Y &&
                   position.Z >= Min.Z &&
                   position.X <= Max.X &&
                   position.Y <= Max.Y &&
                   position.Z <= Max.Z;
		}

        /// <summary>
		/// Returns true if the given AABB overlaps this AABB.
		/// </summary>
		/// <param name="other"></param>
		/// <returns></returns>
        public bool Intersects(Aabb other)
        {
            return Min.X <= other.Max.X &&
                   Max.X >= other.Min.X &&
                   Min.Y <= other.Max.Y &&
                   Max.Y >= other.Min.Y &&
                   Min.Z <= other.Max.Z &&
                   Max.Z >= other.Min.Z;
		}

        public bool Intersects(Ray ray, out float tmin, out float tmax)
		{
			Vector3[] bounds = Bounds;

			tmin = (bounds[ray.Sign.X].X - ray.Origin.X) * ray.InverseDirection.X;
			tmax = (bounds[1 - ray.Sign.X].X - ray.Origin.X) * ray.InverseDirection.X;
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

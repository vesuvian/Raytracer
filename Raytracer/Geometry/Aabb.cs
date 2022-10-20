using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
using Raytracer.Extensions;
using Raytracer.Math;

namespace Raytracer.Geometry
{
    [DebuggerDisplay("Min = {Min}, Max = {Max}")]
    public struct Aabb
    {
        private readonly Vector3[] m_Bounds;

        public Vector3 Min { get { return m_Bounds?[0] ?? Vector3.Zero; } }

        public Vector3 Max { get { return m_Bounds?[1] ?? Vector3.Zero; } }

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
        
        public IEnumerable<Plane> Planes
        {
            get
            {
                yield return new Plane(new Vector3(1, 0, 0), Min.X);
                yield return new Plane(new Vector3(-1, 0, 0), -Max.X);
                yield return new Plane(new Vector3(0, 1, 0), Min.Y);
                yield return new Plane(new Vector3(0, -1, 0), -Max.Y);
                yield return new Plane(new Vector3(0, 0, 1), Min.Z);
                yield return new Plane(new Vector3(0, 0, -1), -Max.Z);
            }
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        public Aabb(Vector3 min, Vector3 max)
        {
            m_Bounds = new[]
            {
                Vector3.Min(min, max),
                Vector3.Max(min, max)
            };
        }

        public static Aabb operator +(Aabb a, Aabb b)
        {
            return new Aabb(Vector3.Min(a.Min, b.Min), Vector3.Max(a.Max, b.Max));
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
                    a = new Aabb(new Vector3(MathF.Min(a.Max.X, b.Max.X), a.Min.Y, a.Min.Z), a.Max);

                // Trim right
                if (b.Max.X >= a.Max.X && b.Min.X <= a.Max.X)
                    a = new Aabb(a.Min, new Vector3(MathF.Max(a.Min.X, b.Min.X), a.Max.Y, a.Max.Z));
            }

            // Slice along Y
            if (b.Min.X <= a.Min.X &&
                b.Max.X >= a.Max.X &&
                b.Min.Z <= a.Min.Z &&
                b.Max.Z >= a.Max.Z)
            {
                // Trim bottom
                if (b.Min.Y <= a.Min.Y && b.Max.Y >= a.Min.Y)
                    a = new Aabb(new Vector3(a.Min.X, MathF.Min(a.Max.Y, b.Max.Y), a.Min.Z), a.Max);

                // Trim right
                if (b.Max.X >= a.Max.X && b.Min.X <= a.Max.X)
                    a = new Aabb(a.Min, new Vector3(a.Max.X, MathF.Max(a.Min.Y, b.Min.Y), a.Max.Z));
			}

            // Slice along Z
            if (b.Min.Y <= a.Min.Y &&
                b.Max.Y >= a.Max.Y &&
                b.Min.X <= a.Min.X &&
                b.Max.X >= a.Max.X)
            {
                // Trim back
                if (b.Min.Z <= a.Min.Z && b.Max.Z >= a.Min.Z)
                    a = new Aabb(new Vector3(a.Min.X, a.Min.Y, MathF.Min(a.Max.Z, b.Max.Z)), a.Max);

                // Trim front
                if (b.Max.Z >= a.Max.Z && b.Min.Z <= a.Max.Z)
                    a = new Aabb(a.Min, new Vector3(a.Max.X, a.Max.Y, MathF.Max(a.Min.Z, b.Min.Z)));
            }

            return a;
        }

		public override string ToString()
		{
			return $"{nameof(Aabb)}(Min = {Min}, Max = {Max})";
		}

		public Aabb Intersection(Aabb other)
        {
            return new Aabb(Vector3.Max(Min, other.Min),
                            Vector3.Min(Max, other.Max));
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

        public bool Intersects(Ray ray, float minDelta, float maxDelta)
        {
            float tMin;
            float tMax;
            if (!Intersects(ray, out tMin, out tMax))
                return false;

            return (!(tMin < minDelta) || !(tMax < minDelta)) &&
                   (!(tMin > maxDelta) || !(tMax > maxDelta));
        }

        public bool Intersects(Ray ray, out float tmin, out float tmax)
        {
            var bounds = m_Bounds ?? new Vector3[2];

            var sign = ray.Sign;
            var origin = ray.Origin;
            var inverseDirection = ray.InverseDirection;

            tmin = (bounds[sign.X].X - origin.X) * inverseDirection.X;
			tmax = (bounds[1 - sign.X].X - origin.X) * inverseDirection.X;
			float tymin = (bounds[sign.Y].Y - origin.Y) * inverseDirection.Y;
			float tymax = (bounds[1 - sign.Y].Y - origin.Y) * inverseDirection.Y;

			if (tmin > tymax || tymin > tmax)
				return false;

			if (tymin > tmin)
				tmin = tymin;
			if (tymax < tmax)
				tmax = tymax;

			float tzmin = (bounds[sign.Z].Z - origin.Z) * inverseDirection.Z;
			float tzmax = (bounds[1 - sign.Z].Z - origin.Z) * inverseDirection.Z;

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

        public bool ClipLine(Vector3 a, Vector3 b, out Vector3 clippedA, out Vector3 clippedB)
        {
	        clippedA = Vector3.Zero;
	        clippedB = Vector3.Zero;

	        Ray ray = new Ray(a, Vector3.Normalize(b - a));

            // Ray doesn't intersect the box
	        float min;
	        float max;
	        if (!Intersects(ray, out min, out max))
		        return false;

	        float originalLength = (b - a).Length();

            // Line doesn't reach the box
	        if (max < 0 || min > originalLength)
		        return false;

	        clippedA = ray.PositionAtDelta(MathF.Max(min, 0));
	        clippedB = ray.PositionAtDelta(MathF.Min(max, originalLength));

	        return true;
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

            return new Aabb(new Vector3(minX, minY, minZ),
                            new Vector3(maxX, maxY, maxZ));
		}
	}
}

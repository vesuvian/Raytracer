using System;
using System.Collections.Generic;
using System.Numerics;
using Raytracer.Math;

namespace Raytracer.SceneObjects.Geometry
{
	public sealed class Triangle : AbstractSceneGeometry
	{
		public Vector3 A { get; set; }
		public Vector3 B { get; set; }
		public Vector3 C { get; set; }

		public static Vector3 GetNormal(Vector3 a, Vector3 b, Vector3 c)
		{
			return Vector3.Cross(b - a, c - a);
		}

		public static bool HitTriangle(Vector3 a, Vector3 b, Vector3 c, Ray ray, out float t)
		{
			t = default;

			// compute plane's normal
			Vector3 n = GetNormal(a, b, c);

			// Step 1: finding P

			// check if ray and plane are parallel ?
			float ndotRayDirection = Vector3.Dot(n, ray.Direction);
			if (MathF.Abs(ndotRayDirection) < 0.00001f)
				return false;

			// compute d parameter using equation 2
			float d = Vector3.Dot(n, a);

			// compute t (equation 3)
			t = (Vector3.Dot(n, ray.Origin) + d) / ndotRayDirection;
			if (t < 0)
				return false;

			// compute the intersection point using equation 1
			Vector3 p = ray.Origin + t * ray.Direction;

			// Step 2: inside-outside test

			// edge 0
			Vector3 edge0 = b - a;
			Vector3 vp0 = p - a;
			var C = Vector3.Cross(edge0, vp0);
			if (Vector3.Dot(n, C) < 0)
				return false;

			// edge 1
			Vector3 edge1 = c - b;
			Vector3 vp1 = p - b;
			C = Vector3.Cross(edge1, vp1);
			if (Vector3.Dot(n, C) < 0)
				return false;

			// edge 2
			Vector3 edge2 = a - c;
			Vector3 vp2 = p - c;
			C = Vector3.Cross(edge2, vp2);
			if (Vector3.Dot(n, C) < 0)
				return false;

			return true;
        }

		public override IEnumerable<Intersection> GetIntersections(Ray ray)
		{
			// First transform the ray into local space
			ray = ray.Multiply(WorldToLocal);

			float t;
			if (!HitTriangle(A, B, C, ray, out t))
				yield break;

			yield return new Intersection
			{
				Position = ray.PositionAtDelta(t),
				Normal = GetNormal(A, B, C),
				RayOrigin = ray.Origin
			}.Multiply(LocalToWorld);
		}
	}
}

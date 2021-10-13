using System;
using System.Collections.Generic;
using System.Numerics;
using Raytracer.Math;

namespace Raytracer.SceneObjects.Geometry
{
	public sealed class Triangle : AbstractSceneGeometry
	{
		private Vector3 m_A;
		private Vector3 m_B;
		private Vector3 m_C;

		public Vector3 A
		{
			get { return m_A; }
			set
			{
				m_A = value;
				// Force a rebuild of the AABB
				HandleTransformChange();
			}
		}

		public Vector3 B
		{
			get { return m_B; }
			set
			{
				m_B = value;
				// Force a rebuild of the AABB
				HandleTransformChange();
			}
		}

		public Vector3 C
		{
			get { return m_C; }
			set
			{
				m_C = value;
				// Force a rebuild of the AABB
				HandleTransformChange();
			}
		}

		public static Vector3 GetNormal(Vector3 a, Vector3 b, Vector3 c)
		{
			return Vector3.Normalize(Vector3.Cross(b - a, c - a));
		}

		public static Vector3 GetInterpolatedVertexNormal(Vector3 a, Vector3 b, Vector3 c, float u, float v)
		{
			return (1 - u - v) * a + u * b + v * c;
		}

		public static Vector2 GetInterpolatedVertexUv(Vector2 a, Vector2 b, Vector2 c, float u, float v)
		{
			return (1 - u - v) * a + u * b + v * c;
		}

		public static float GetSurfaceArea(Vector3 a, Vector3 b, Vector3 c)
		{
			Vector3 ab = b - a;
			Vector3 ac = c - a;
			Vector3 bc = c - b;

			float lengthA = ab.Length();
			float lengthB = ac.Length();
			float lengthC = bc.Length();

			float abcOver2 = (lengthA + lengthB + lengthC) / 2;

			return MathF.Sqrt(abcOver2 * (abcOver2 - lengthA) * (abcOver2 - lengthB) * (abcOver2 - lengthC));
		}

		public static bool HitTriangle(Vector3 a, Vector3 b, Vector3 c, Ray ray, out float t, out float u, out float v)
		{
			t = default;
			u = default;
			v = default;

			// Get the plane normal
			Vector3 ab = b - a;
			Vector3 ac = c - a;
			Vector3 pvec = Vector3.Cross(ray.Direction, ac);

			// Ray and triangle are parallel if det is close to 0
			float det = Vector3.Dot(ab, pvec);
			if (MathF.Abs(det) < 0.00001f)
				return false;

			float invDet = 1 / det;

			Vector3 tvec = ray.Origin - a;
			u = Vector3.Dot(tvec, pvec) * invDet;
			if (u < 0 || u > 1)
				return false;

			Vector3 qvec = Vector3.Cross(tvec, ab);
			v = Vector3.Dot(ray.Direction, qvec) * invDet;
			if (v < 0 || u + v > 1)
				return false;

			t = Vector3.Dot(ac, qvec) * invDet;
			return true;
		}

		protected override IEnumerable<Intersection> GetIntersectionsFinal(Ray ray)
		{
			// First transform the ray into local space
			ray = ray.Multiply(WorldToLocal);

			float t, u, v;
			if (!HitTriangle(A, B, C, ray, out t, out u, out v))
				yield break;

			Vector3 normal = GetNormal(A, B, C);

			yield return new Intersection
			{
				Position = ray.PositionAtDelta(t),
				Normal = normal,
				Tangent = Vector3.Normalize(B - A),
				Bitangent = Vector3.Normalize(C - A),
				Ray = ray,
				Uv = new Vector2(u, v),
                Geometry = this,
                Material = Material
			}.Multiply(LocalToWorld);
		}

		protected override float CalculateUnscaledSurfaceArea()
		{
			return GetSurfaceArea(A, B, C);
		}

		protected override Aabb CalculateAabb()
		{
			return Aabb.FromPoints(LocalToWorld, A, B, C);
		}
	}
}

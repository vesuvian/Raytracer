using System.Collections.Generic;
using System.Numerics;
using Raytracer.Geometry;
using Raytracer.Math;
using Raytracer.Parsers;

namespace Raytracer.SceneObjects.Geometry.Primitives
{
	public sealed class TriangleSceneGeometry : AbstractSceneGeometry
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

        protected override IEnumerable<Intersection> GetIntersectionsFinal(Ray ray)
		{
			// First transform the ray into local space
			ray = ray.Multiply(WorldToLocal);

			float t, u, v;
			if (!Triangle.HitTriangle(A, B, C, ray, out t, out u, out v))
				yield break;

			Vector3 normal = Triangle.GetNormal(A, B, C);

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
			return Triangle.GetSurfaceArea(A, B, C);
		}

		protected override Aabb CalculateAabb()
		{
			return Aabb.FromPoints(LocalToWorld, A, B, C);
		}
	}
}

using System;
using System.Collections.Generic;
using System.Numerics;
using Raytracer.Geometry;
using Raytracer.Math;

namespace Raytracer.SceneObjects.Geometry.Primitives
{
	public sealed class QuadSceneGeometry : AbstractSceneGeometry
	{
		private static readonly Vector3 s_Normal = new Vector3(0, 1, 0);

		private float m_Edge = 1.0f;

		public float Edge
		{
			get
			{
				return m_Edge;
			}
			set
			{
				m_Edge = value;
				// Force a rebuild of the AABB
				HandleTransformChange();
			}
		}

		protected override bool GetIntersectionFinal(Ray ray, out Intersection intersection, float minDelta = float.NegativeInfinity,
		                                             float maxDelta = float.PositiveInfinity)
		{
			intersection = default;

			// First transform the ray into local space
			ray = ray.Multiply(WorldToLocal);

			float t;
			if (!PlaneSceneGeometry.HitPlane(ray, out t))
				return false;

			Vector3 position = ray.PositionAtDelta(t);
			if (MathF.Abs(position.X) > Edge / 2 ||
			    MathF.Abs(position.Z) > Edge / 2)
				return false;

			Vector2 uv = (new Vector2(position.X / Edge, position.Z) / Edge) + (Vector2.One / 2);

			intersection = new Intersection
			{
				Normal = s_Normal,
				Tangent = new Vector3(1, 0, 0),
				Bitangent = new Vector3(0, 0, 1),
				Position = position,
				Ray = ray,
				Uv = uv,
                Geometry = this,
                Material = Material
			}.Multiply(LocalToWorld);

			return intersection.RayDelta >= minDelta && intersection.RayDelta <= maxDelta;
		}

		protected override float CalculateUnscaledSurfaceArea()
		{
			return m_Edge * m_Edge;
		}

		protected override Aabb CalculateAabb()
		{
			return Aabb.FromPoints(LocalToWorld,
			                       new Vector3(-Edge, 0, -Edge) / 2,
			                       new Vector3(-Edge, 0, Edge) / 2,
			                       new Vector3(Edge, 0, -Edge) / 2,
			                       new Vector3(Edge, 0, Edge) / 2);
		}
	}
}

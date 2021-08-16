using System;
using System.Collections.Generic;
using System.Numerics;
using Raytracer.Math;

namespace Raytracer.SceneObjects.Geometry
{
	public sealed class Quad : AbstractSceneGeometry
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

		public override IEnumerable<Intersection> GetIntersectionsFinal(Ray ray)
		{
			// First transform the ray into local space
			ray = ray.Multiply(WorldToLocal);

			float t;
			if (!Plane.HitPlane(ray, out t))
				yield break;

			Vector3 position = ray.PositionAtDelta(t);
			if (MathF.Abs(position.X) > Edge / 2 ||
			    MathF.Abs(position.Z) > Edge / 2)
				yield break;

			Vector2 uv = (new Vector2(position.X / Edge, position.Z) / Edge) + (Vector2.One / 2);

			yield return new Intersection
			{
				Normal = s_Normal,
				Tangent = new Vector3(1, 0, 0),
				Bitangent = new Vector3(0, 0, 1),
				Position = position,
				RayOrigin = ray.Origin,
				Uv = uv
			}.Multiply(LocalToWorld);
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

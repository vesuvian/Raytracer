using System;
using System.Collections.Generic;
using System.Numerics;
using Raytracer.Math;

namespace Raytracer.SceneObjects.Geometry
{
	public sealed class Disc : AbstractSceneGeometry
	{
		private static readonly Vector3 s_Normal = new Vector3(0, 1, 0);

		private float m_Radius = 0.5f;

		public float Radius
		{
			get
			{
				return m_Radius;
			}
			set
			{
				m_Radius = value;
				// Force a rebuild of the AABB
				HandleTransformChange();
			}
		}

		protected override IEnumerable<Intersection> GetIntersectionsFinal(Ray ray)
		{
			// First transform the ray into local space
			ray = ray.Multiply(WorldToLocal);

			float t;
			if (!Plane.HitPlane(ray, out t))
				yield break;

			Vector3 position = ray.PositionAtDelta(t);
			if (position.Length() > Radius)
				yield break;

			Vector2 uv = (new Vector2(position.X / Radius, position.Z / Radius) + Vector2.One) / 2;

			yield return new Intersection
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
		}

		protected override float CalculateUnscaledSurfaceArea()
		{
			return MathF.PI * m_Radius * m_Radius;
		}

		protected override Aabb CalculateAabb()
		{
			return Aabb.FromPoints(LocalToWorld,
			                       new Vector3(-m_Radius, 0, -m_Radius),
			                       new Vector3(-m_Radius, 0, m_Radius),
			                       new Vector3(m_Radius, 0, -m_Radius),
			                       new Vector3(m_Radius, 0, m_Radius));
		}
	}
}

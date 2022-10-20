using System;
using System.Collections.Generic;
using System.Numerics;
using Raytracer.Geometry;
using Raytracer.Math;

namespace Raytracer.SceneObjects.Geometry.Primitives
{
	public sealed class SphereSceneGeometry : AbstractSceneGeometry
	{
		private float m_Radius = 1.0f;

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

		public static IEnumerable<float> HitSphere(Vector3 origin, float radius, Ray ray)
		{
			Vector3 oc = ray.Origin - origin;
			float a = Vector3.Dot(ray.Direction, ray.Direction);
			float b = 2.0f * Vector3.Dot(oc, ray.Direction);
			float c = Vector3.Dot(oc, oc) - radius * radius;

			float discriminant = b * b - 4 * a * c;
			if (discriminant < 0)
				yield break;

			float t1 = (-b - (float)System.Math.Sqrt(discriminant)) / (2.0f * a);
			float t2 = (-b + (float)System.Math.Sqrt(discriminant)) / (2.0f * a);

			yield return t1;
			yield return t2;
		}

		protected override bool GetIntersectionFinal(Ray ray, out Intersection intersection, float minDelta = float.NegativeInfinity,
		                                             float maxDelta = float.PositiveInfinity)
		{
			intersection = default;

			// First transform the ray into local space
			ray = ray.Multiply(WorldToLocal);

			// Find the intersects
			float bestT = float.MaxValue;
			bool found = false;
			foreach (float t in HitSphere(Vector3.Zero, Radius, ray))
			{
				Vector3 position = ray.PositionAtDelta(t);
				Vector3 normal = Vector3.Normalize(MathF.Sign(Radius) * position);
				Vector3 tangent =
					normal == new Vector3(0, 1, 0) || normal == new Vector3(0, -1, 0)
						? new Vector3(1, 0, 0)
						: Vector3.Normalize(Vector3.Cross(normal, new Vector3(0, 1, 0)));
				Vector3 bitangent = Vector3.Normalize(Vector3.Cross(tangent, normal));
				Vector2 uv = GetUv(position);

				Intersection thisIntersection = new Intersection
				{
					Position = position,
					Tangent = tangent,
					Bitangent = bitangent,
					Normal = normal,
					Ray = ray,
					Uv = uv,
                    Geometry = this,
                    Material = Material
				}.Multiply(LocalToWorld);

				if (t < minDelta || t > maxDelta)
					continue;

				if (t > bestT)
					continue;

				bestT = t;
				found = true;
				intersection = thisIntersection;
			}

			return found;
		}

		protected override float CalculateUnscaledSurfaceArea()
		{
			return 4 * MathF.PI * m_Radius * m_Radius;
		}

		protected override Aabb CalculateAabb()
		{
			return new Aabb
			(
				new Vector3(-m_Radius),
				new Vector3(m_Radius)
			).Multiply(LocalToWorld);
		}

		public static Vector2 GetUv(Vector3 position)
		{
			position = Vector3.Normalize(position);

			float u = MathF.Atan2(position.X, position.Z) / (2.0f * MathF.PI) + 0.5f;
			float v = position.Y * 0.5f + 0.5f;

			return new Vector2(u, v);
		}
	}
}

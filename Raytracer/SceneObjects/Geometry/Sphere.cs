using System;
using System.Collections.Generic;
using System.Numerics;
using Raytracer.Math;

namespace Raytracer.SceneObjects.Geometry
{
	public sealed class Sphere : AbstractSceneGeometry
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

		protected override IEnumerable<Intersection> GetIntersectionsFinal(Ray ray)
		{
			// First transform the ray into local space
			ray = ray.Multiply(WorldToLocal);

			// Find the intersects
			foreach (float intersect in HitSphere(Vector3.Zero, Radius, ray))
			{
				Vector3 position = ray.PositionAtDelta(intersect);
				Vector3 normal = Vector3.Normalize(MathF.Sign(Radius) * position);
				Vector3 tangent =
					normal == new Vector3(0, 1, 0) || normal == new Vector3(0, -1, 0)
						? new Vector3(1, 0, 0)
						: Vector3.Normalize(Vector3.Cross(normal, new Vector3(0, 1, 0)));
				Vector3 bitangent = Vector3.Normalize(Vector3.Cross(tangent, normal));
				Vector2 uv = GetUv(position);

				yield return new Intersection
				{
					Position = position,
					Tangent = tangent,
					Bitangent = bitangent,
					Normal = normal,
					RayOrigin = ray.Origin,
					Uv = uv
				}.Multiply(LocalToWorld);
			}
		}

		protected override Aabb CalculateAabb()
		{
			return new Aabb
			{
				Min = new Vector3(-m_Radius),
				Max = new Vector3(m_Radius)
			}.Multiply(LocalToWorld);
		}

		private static Vector2 GetUv(Vector3 position)
		{
			position = Vector3.Normalize(position);

			float u = MathF.Atan2(position.X, position.Z) / (2.0f * MathF.PI) + 0.5f;
			float v = position.Y * 0.5f + 0.5f;

			return new Vector2(u, v);
		}
	}
}

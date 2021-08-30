using System;
using System.Collections.Generic;
using System.Numerics;
using Raytracer.Math;

namespace Raytracer.SceneObjects.Geometry
{
	public sealed class Cylinder : AbstractSceneGeometry
	{
		private float m_Radius = 1.0f;
		private float m_Height = 1.0f;

		public float Height
		{
			get
			{
				return m_Height;
			}
			set
			{
				m_Height = value;
				// Force a rebuild of the AABB
				HandleTransformChange();
			}
		}

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

		protected override Aabb CalculateAabb()
		{
			return new Aabb
			{
				Min = new Vector3(-Radius, -Height / 2, -Radius),
				Max = new Vector3(Radius, Height / 2, Radius)
			}.Multiply(LocalToWorld);
		}

		protected override IEnumerable<Intersection> GetIntersectionsFinal(Ray ray)
		{
			// First transform ray to local space
			ray = ray.Multiply(WorldToLocal);

			// Get the intersections along the curved surface
			float a = ray.Direction.X * ray.Direction.X + ray.Direction.Z * ray.Direction.Z;
			float b = 2 * (ray.Direction.X * ray.Origin.X + ray.Direction.Z * ray.Origin.Z);
			float c = ray.Origin.X * ray.Origin.X + ray.Origin.Z * ray.Origin.Z - Radius * Radius;

			float delta = b * b - 4 * (a * c);
			if (MathF.Abs(delta) < 0.001f)
				yield break;
			if (delta < 0.0f)
				yield break;

			float t1 = (-b - MathF.Sqrt(delta)) / (2 * a);
			float t2 = (-b + MathF.Sqrt(delta)) / (2 * a);
			Vector3 position1 = ray.PositionAtDelta(t1);
			Vector3 position2 = ray.PositionAtDelta(t2);
			Vector3 bitangent = Vector3.UnitY;

			if (position1.Y <= Height / 2 && position1.Y >= -Height / 2)
			{
				Vector3 normal = Vector3.Normalize(new Vector3(position1.X, 0, position1.Z));
				Vector3 tangent = Vector3.Cross(normal, bitangent);
				Vector2 uv = CalculateCylinderUv(position1);

				yield return new Intersection
				{
					Position = position1,
					Tangent = tangent,
					Bitangent = bitangent,
					Normal = normal,
					Ray = ray,
					Uv = uv
				}.Multiply(LocalToWorld);
			}

			if (position2.Y <= Height / 2 && position2.Y >= -Height / 2)
			{
				Vector3 normal = Vector3.Normalize(new Vector3(position2.X, 0, position2.Z));
				Vector3 tangent = Vector3.Cross(normal, bitangent);
				Vector2 uv = CalculateCylinderUv(position2);

				yield return new Intersection
				{
					Position = position2,
					Tangent = tangent,
					Bitangent = bitangent,
					Normal = normal,
					Ray = ray,
					Uv = uv
				}.Multiply(LocalToWorld);
			}

			// Top cap
			Ray topCapRay = ray;
			topCapRay.Origin += Vector3.UnitY * Height / 2;
			float t;
			if (Plane.HitPlane(topCapRay, out t))
			{
				Vector3 position = topCapRay.PositionAtDelta(t);

				if (position.Length() <= Radius)
				{
					position -= Vector3.UnitY * Height / 2;
					Vector2 uv = (new Vector2(position.X / Radius, position.Z / Radius) + Vector2.One) / 2;

					yield return new Intersection
					{
						Normal = Vector3.UnitY,
						Tangent = new Vector3(1, 0, 0),
						Bitangent = new Vector3(0, 0, 1),
						Position = position,
						Ray = ray,
						Uv = uv
					}.Multiply(LocalToWorld);
				}
			}

			// Bottom cap
			Ray bottomCapRay = ray;
			bottomCapRay.Origin -= Vector3.UnitY * Height / 2;
			if (Plane.HitPlane(bottomCapRay, out t))
			{
				Vector3 position = bottomCapRay.PositionAtDelta(t);

				if (position.Length() <= Radius)
				{
					position += Vector3.UnitY * Height / 2;
					Vector2 uv = (new Vector2(position.X / Radius, position.Z / Radius) + Vector2.One) / 2;

					yield return new Intersection
					{
						Normal = -Vector3.UnitY,
						Tangent = new Vector3(1, 0, 0),
						Bitangent = new Vector3(0, 0, -1),
						Position = position,
						Ray = ray,
						Uv = uv
					}.Multiply(LocalToWorld);
				}
			}
		}

		protected override float CalculateUnscaledSurfaceArea()
		{
			return (2 * MathF.PI * m_Radius * m_Height) + (2 * MathF.PI * m_Radius * m_Radius);
		}

		private Vector2 CalculateCylinderUv(Vector3 position)
		{
			position += Vector3.UnitY * Height / 2;
			float v = position.Y / Height;

			float dot = Vector3.Dot(new Vector3(0, 0, 1), Vector3.Normalize(new Vector3(position.X, 0, position.Z)));
			
			float angle = MathF.Acos(dot);
			if (position.X > 0)
				angle = 2 * MathF.PI - angle;

			float u = angle / (2 * MathF.PI);

			return new Vector2(u, v);
		}
	}
}

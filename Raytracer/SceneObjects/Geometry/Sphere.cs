using System.Collections.Generic;
using System.Numerics;
using Raytracer.Math;

namespace Raytracer.SceneObjects.Geometry
{
	public sealed class Sphere : AbstractSceneGeometry
	{
		public float Radius { get; set; }

		public Sphere()
		{
			Radius = 1;
		}

		private static bool HitSphere(float radius, Ray ray, out float t)
		{
			t = default;

			Vector3 oc = ray.Origin;
			float a = Vector3.Dot(ray.Direction, ray.Direction);
			float b = 2.0f * Vector3.Dot(oc, ray.Direction);
			float c = Vector3.Dot(oc, oc) - radius * radius;

			float discriminant = b * b - 4 * a * c;
			if (discriminant < 0)
				return false;

			t = (-b - (float)System.Math.Sqrt(discriminant)) / (2.0f * a);
			return t >= 0;
		}

		public override IEnumerable<Intersection> GetIntersections(Ray ray)
		{
			// First transform the ray into local space
			ray = ray.Multiply(WorldToLocal);

			// Find the intersect
			// TODO - Find both intersections
			float t;
			if (!HitSphere(Radius, ray, out t))
				yield break;

			Vector3 position = ray.PositionAtDelta(t);
			Vector3 normal = Vector3.Normalize(position);

			yield return new Intersection
			{
				Position = position,
				Normal = normal,
				RayOrigin = ray.Origin
			}.Multiply(LocalToWorld);
		}
	}
}

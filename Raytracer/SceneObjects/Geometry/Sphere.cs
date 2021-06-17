using System.Numerics;
using Raytracer.Math;

namespace Raytracer.SceneObjects.Geometry
{
	public sealed class Sphere : AbstractGeometry
	{
		public float Radius { get; set; }

		public Sphere()
		{
			Radius = 1;
		}

		private static bool HitSphere(Vector3 center, float radius, Ray r, out float t)
		{
			t = default;

			Vector3 oc = r.Origin - center;
			float a = Vector3.Dot(r.Direction, r.Direction);
			float b = 2.0f * Vector3.Dot(oc, r.Direction);
			float c = Vector3.Dot(oc, oc) - radius * radius;

			float discriminant = b * b - 4 * a * c;
			if (discriminant < 0)
				return false;

			t = (-b - (float)System.Math.Sqrt(discriminant)) / (2.0f * a);
			return true;
		}

		public override bool GetIntersection(Ray ray, out Intersection intersection)
		{
			intersection = default;

			// First transform the ray into local space
			ray = ray.Multiply(WorldToLocal);

			// Find the intersect
			float t;
			return HitSphere(Vector3.Zero, Radius, ray, out t);

			//		Vector3 position = t0 < t1 ? Position + ray.Direction * t0 : Position + ray.Direction * t1;
			//		Vector3 normal = Vector3.Normalize(position);
			//		
			//		HitSphere(Position, Radius, ray).Dump();
			//
			//		// Transform back into world space
			//		position = MultiplyPoint(position, Transform);
			//		normal = MultiplyDirection(normal, Transform);
			//		normal = Vector3.Normalize(normal);
			//
			//		intersection = new Intersection
			//		{
			//			Position = position,
			//			Normal = normal,
			//			Distance = Vector3.Distance(ray.Origin, position)
			//		};
			//		
			//		return true;
		}
	}
}

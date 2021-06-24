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

		public static IEnumerable<float> HitSphere(float radius, Ray ray)
		{
			Vector3 oc = ray.Origin;
			float a = Vector3.Dot(ray.Direction, ray.Direction);
			float b = 2.0f * Vector3.Dot(oc, ray.Direction);
			float c = Vector3.Dot(oc, oc) - radius * radius;

			float discriminant = b * b - 4 * a * c;
			if (discriminant < 0)
				yield break;

			float t1 = (-b - (float)System.Math.Sqrt(discriminant)) / (2.0f * a);
			float t2 = (-b + (float)System.Math.Sqrt(discriminant)) / (2.0f * a);

			if (t1 > 0)
				yield return t1;

			if (t2 > 0 && System.Math.Abs(t1 - t2) > 0.00001f)
				yield return t2;
		}

		public override IEnumerable<Intersection> GetIntersections(Ray ray)
		{
			// First transform the ray into local space
			ray = ray.Multiply(WorldToLocal);

			// Find the intersects
			foreach (float intersect in HitSphere(Radius, ray))
			{
				Vector3 position = ray.PositionAtDelta(intersect);
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
}

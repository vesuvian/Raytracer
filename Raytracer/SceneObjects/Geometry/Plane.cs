using System;
using System.Numerics;
using Raytracer.Math;

namespace Raytracer.SceneObjects.Geometry
{
	public sealed class Plane : AbstractGeometry
	{
		private static readonly Vector3 s_Normal = new Vector3(0, 1, 0);

		public override bool GetIntersection(Ray ray, out Intersection intersection)
		{
			intersection = default;

			// First transform the ray into local space
			ray = ray.Multiply(WorldToLocal);

			float denom = Vector3.Dot(s_Normal, ray.Direction);
			if (MathF.Abs(denom) <= 0.00001f)
				return false;

			float t = -Vector3.Dot(s_Normal, ray.Origin) / denom;
			if (t <= 0.00001f)
				return false;

			Vector3 position = ray.Origin + t * ray.Direction;

			// Plane normal is flipped if the ray comes from behind
			Vector3 normal = Vector3.Dot(s_Normal, ray.Direction) < 0 ? s_Normal : s_Normal * -1;

			intersection = new Intersection
			{
				Normal = normal,
				Position = position,
				RayOrigin = ray.Origin
			};

			// Transform intersection from local to world space
			intersection = intersection.Multiply(LocalToWorld);
			return true;
		}
	}
}

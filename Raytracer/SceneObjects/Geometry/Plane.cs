using System;
using System.Numerics;
using Raytracer.Math;
using Raytracer.Utils;

namespace Raytracer.SceneObjects.Geometry
{
	public sealed class Plane : AbstractGeometry
	{
		private static Vector3 VECTOR3_UP = new Vector3(0,1,0);

		public override bool GetIntersection(Ray ray, out Intersection intersection)
		{
			intersection = default;

			// First transform the ray into local space
			ray = ray.Multiply(WorldToLocal);

			float vdot = Vector3.Dot(ray.Direction, VECTOR3_UP);
			float ndot = -Vector3.Dot(ray.Origin, VECTOR3_UP);

			if (MathUtils.Approximately(vdot, 0.0f))
				return false;

			float t = ndot / vdot;
			if (t <= 0.0f)
				return false;

			intersection = new Intersection
			{
				Normal = VECTOR3_UP,
				Position = ray.Direction * MathF.Abs(ndot) + ray.Origin,
				RayOrigin = ray.Origin
			};

			// Transform intersection from local to world space
			intersection = intersection.Multiply(LocalToWorld);
			return true;
		}
	}
}

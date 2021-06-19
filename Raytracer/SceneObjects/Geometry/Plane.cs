using System;
using System.Numerics;
using Raytracer.Math;
using Raytracer.Utils;

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

			float vdot = Vector3.Dot(ray.Direction, s_Normal);
			float ndot = -Vector3.Dot(ray.Origin, s_Normal);

			if (MathUtils.Approximately(vdot, 0.0f))
				return false;

			float t = ndot / vdot;
			if (t <= 0.0f)
				return false;

			// Plane normal is flipped if the ray comes from behind
			Vector3 normal = Vector3.Dot(s_Normal, ray.Direction) < 0 ? s_Normal : s_Normal * -1;

			intersection = new Intersection
			{
				Normal = normal,
				Position = ray.Direction * MathF.Abs(ndot) + ray.Origin,
				RayOrigin = ray.Origin
			};

			// Transform intersection from local to world space
			intersection = intersection.Multiply(LocalToWorld);
			return true;
		}
	}
}

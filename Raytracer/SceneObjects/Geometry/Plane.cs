using System;
using System.Collections.Generic;
using System.Numerics;
using Raytracer.Math;

namespace Raytracer.SceneObjects.Geometry
{
	public sealed class Plane : AbstractSceneGeometry
	{
		private static readonly Vector3 s_Normal = new Vector3(0, 1, 0);

		public static bool HitPlane(Ray ray, out float t)
		{
			t = default;

			float denom = Vector3.Dot(s_Normal, ray.Direction);
			if (MathF.Abs(denom) <= 0.00001f)
				return false;

			t = -Vector3.Dot(s_Normal, ray.Origin) / denom;
			return t > 0.00001f;
		}

		public override IEnumerable<Intersection> GetIntersections(Ray ray)
		{
			// First transform the ray into local space
			ray = ray.Multiply(WorldToLocal);

			float t;
			if (!HitPlane(ray, out t))
				yield break;

			Vector3 position = ray.PositionAtDelta(t);

			// Plane normal is flipped if the ray comes from behind
			Vector3 normal = Vector3.Dot(s_Normal, ray.Direction) < 0 ? s_Normal : s_Normal * -1;

			yield return new Intersection
			{
				Normal = normal,
				Position = position,
				RayOrigin = ray.Origin,
				Uv = new Vector2(position.X, position.Z)
			}.Multiply(LocalToWorld);
		}

		/// <summary>
		/// Returns the shortest distance from the plane to the given position.
		/// </summary>
		/// <param name="planePosition"></param>
		/// <param name="planeNormal"></param>
		/// <param name="position"></param>
		/// <param name="closestPosition"></param>
		/// <returns></returns>
		public static float Distance(Vector3 planePosition, Vector3 planeNormal, Vector3 position, out Vector3 closestPosition)
		{
			float sn = -Vector3.Dot(planeNormal, (position - planePosition));
			float sd = Vector3.Dot(planeNormal, planeNormal);
			var sb = sn / sd;

			closestPosition = position + sb * planeNormal;
			return Vector3.Distance(position, closestPosition);
		}
	}
}

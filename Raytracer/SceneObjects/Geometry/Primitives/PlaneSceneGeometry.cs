using System;
using System.Collections.Generic;
using System.Numerics;
using Raytracer.Geometry;
using Raytracer.Math;

namespace Raytracer.SceneObjects.Geometry.Primitives
{
	public sealed class PlaneSceneGeometry : AbstractSceneGeometry
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

		protected override bool GetIntersectionFinal(Ray ray, out Intersection intersection, float minDelta = float.NegativeInfinity,
		                                             float maxDelta = float.PositiveInfinity)
		{
			intersection = default;

			// First transform the ray into local space
			ray = ray.Multiply(WorldToLocal);

			float t;
			if (!HitPlane(ray, out t))
				return false;

			Vector3 position = ray.PositionAtDelta(t);

			intersection = new Intersection
			{
				Normal = s_Normal,
				Tangent = new Vector3(1, 0, 0),
				Bitangent = new Vector3(0, 0, 1),
				Position = position,
				Ray = ray,
				Uv = new Vector2(position.X, position.Z),
                Geometry = this,
                Material = Material
			}.Multiply(LocalToWorld);

			return intersection.RayDelta >= minDelta && intersection.RayDelta <= maxDelta;
		}

		protected override float CalculateUnscaledSurfaceArea()
		{
			return float.PositiveInfinity;
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

		protected override Aabb CalculateAabb()
		{
			return new Aabb {Min = new Vector3(float.NegativeInfinity), Max = new Vector3(float.PositiveInfinity)};
		}
	}
}

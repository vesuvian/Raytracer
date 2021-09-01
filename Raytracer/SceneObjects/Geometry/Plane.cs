using System;
using System.Collections.Generic;
using System.Numerics;
using Raytracer.Extensions;
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

		public override Vector3 GetRandomPointOnSurface(Random random = null)
		{
			random ??= new Random();
			float x = random.NextFloat(float.MinValue, float.MaxValue);
			float z = random.NextFloat(float.MinValue, float.MaxValue);
			Vector3 output = new Vector3(x, 0, z);
			return LocalToWorld.MultiplyPoint(output);
		}

		protected override IEnumerable<Intersection> GetIntersectionsFinal(Ray ray)
		{
			// First transform the ray into local space
			ray = ray.Multiply(WorldToLocal);

			float t;
			if (!HitPlane(ray, out t))
				yield break;

			Vector3 position = ray.PositionAtDelta(t);

			yield return new Intersection
			{
				Normal = s_Normal,
				Tangent = new Vector3(1, 0, 0),
				Bitangent = new Vector3(0, 0, 1),
				Position = position,
				Ray = ray,
				Uv = new Vector2(position.X, position.Z)
			}.Multiply(LocalToWorld);
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

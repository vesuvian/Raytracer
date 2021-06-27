using System;
using System.Collections.Generic;
using System.Numerics;
using Raytracer.Math;

namespace Raytracer.SceneObjects.Geometry
{
	public sealed class Quad : AbstractSceneGeometry
	{
		public float Edge { get; set; } = 1;

		private static readonly Vector3 s_Normal = new Vector3(0, 1, 0);

		public override IEnumerable<Intersection> GetIntersections(Ray ray)
		{
			// First transform the ray into local space
			ray = ray.Multiply(WorldToLocal);

			float t;
			if (!Plane.HitPlane(ray, out t))
				yield break;

			Vector3 position = ray.PositionAtDelta(t);
			if (MathF.Abs(position.X) > Edge / 2 ||
			    MathF.Abs(position.Z) > Edge / 2)
				yield break;

			// Plane normal is flipped if the ray comes from behind
			Vector3 normal = Vector3.Dot(s_Normal, ray.Direction) < 0 ? s_Normal : s_Normal * -1;

			Vector2 uv = (new Vector2(position.X / Edge, position.Z) / Edge) + (Vector2.One / 2);

			yield return new Intersection
			{
				Normal = normal,
				Position = position,
				RayOrigin = ray.Origin,
				Uv = uv
			}.Multiply(LocalToWorld);
		}
	}
}

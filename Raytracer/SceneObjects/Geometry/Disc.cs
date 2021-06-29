using System.Collections.Generic;
using System.Numerics;
using Raytracer.Math;

namespace Raytracer.SceneObjects.Geometry
{
	public sealed class Disc : AbstractSceneGeometry
	{
		public float Radius { get; set; } = 0.5f;

		private static readonly Vector3 s_Normal = new Vector3(0, 1, 0);

		public override IEnumerable<Intersection> GetIntersections(Ray ray)
		{
			// First transform the ray into local space
			ray = ray.Multiply(WorldToLocal);

			float t;
			if (!Plane.HitPlane(ray, out t))
				yield break;

			Vector3 position = ray.PositionAtDelta(t);
			if (position.Length() > Radius)
				yield break;

			// Plane normal is flipped if the ray comes from behind
			Vector3 normal = Vector3.Dot(s_Normal, ray.Direction) < 0 ? s_Normal : s_Normal * -1;

			Vector2 uv = (new Vector2(position.X / Radius, position.Z / Radius) + Vector2.One) / 2;

			yield return new Intersection
			{
				Normal = normal,
				Tangent = new Vector3(1, 0, 0),
				Bitangent = new Vector3(0, 0, 1),
				Position = position,
				RayOrigin = ray.Origin,
				Uv = uv
			}.Multiply(LocalToWorld);
		}
	}
}

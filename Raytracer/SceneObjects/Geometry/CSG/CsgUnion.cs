using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Raytracer.Math;

namespace Raytracer.SceneObjects.Geometry.CSG
{
	public sealed class CsgUnion : AbstractCsg
	{
		protected override IEnumerable<Intersection> GetIntersectionsFinal(Ray ray)
		{
			Intersection[] intersections =
				(A?.GetIntersections(ray) ?? Enumerable.Empty<Intersection>())
				.Concat(B?.GetIntersections(ray) ?? Enumerable.Empty<Intersection>())
				.OrderBy(i => i.RayDelta)
				.ToArray();

			int depth = 0;
			
			for (int i = 0; i < intersections.Length; i++)
			{
				Intersection intersection = intersections[i];
				float faceAmount = Vector3.Dot(ray.Direction, intersection.Normal);

				// Enter
				if (faceAmount <= 0)
				{
					if (depth == 0)
						yield return intersection;
					depth++;
				}
				else
				// Exit
				{
					depth--;
					if (depth == 0)
						yield return intersection;
				}
			}
		}

		protected override Aabb CalculateAabb()
		{
			if (A == null && B == null)
				return default;

			if (A == null)
				return B.Aabb;

			if (B == null)
				return A.Aabb;

			return A.Aabb + B.Aabb;
		}
	}
}

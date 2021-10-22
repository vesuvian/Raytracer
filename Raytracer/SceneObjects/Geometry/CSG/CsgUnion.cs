using System;
using Raytracer.Geometry;
using Raytracer.Math;

namespace Raytracer.SceneObjects.Geometry.CSG
{
	public sealed class CsgUnion : AbstractCsg
	{
		protected override bool GetIntersectionFinal(Ray ray, out Intersection intersection, float minDelta = float.NegativeInfinity,
		                                             float maxDelta = float.PositiveInfinity)
		{
			throw new NotImplementedException();

			//Intersection[] intersections =
			//	(A?.GetIntersections(ray, eRayMask.All) ?? Enumerable.Empty<Intersection>())
			//	.Concat(B?.GetIntersections(ray, eRayMask.All) ?? Enumerable.Empty<Intersection>())
			//	.OrderBy(i => i.RayDelta)
			//	.ToArray();

			//int depth = 0;
			
			//for (int i = 0; i < intersections.Length; i++)
			//{
			//	Intersection intersection = intersections[i];
			//	float faceAmount = Vector3.Dot(ray.Direction, intersection.Normal);

			//	// Enter
			//	if (faceAmount <= 0)
			//	{
			//		if (depth == 0)
			//			yield return intersection;
			//		depth++;
			//	}
			//	else
			//	// Exit
			//	{
			//		depth--;
			//		if (depth == 0)
			//			yield return intersection;
			//	}
			//}
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

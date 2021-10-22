using System;
using Raytracer.Geometry;
using Raytracer.Math;

namespace Raytracer.SceneObjects.Geometry.CSG
{
	public sealed class CsgIntersection : AbstractCsg
	{
		protected override bool GetIntersectionFinal(Ray ray, out Intersection intersection, float minDelta = float.NegativeInfinity,
		                                             float maxDelta = float.PositiveInfinity)
		{
			throw new NotImplementedException();

			//Intersection[] aIntersections = (A?.GetIntersections(ray, eRayMask.All) ?? Enumerable.Empty<Intersection>()).ToArray();
			//Intersection[] bIntersections = (B?.GetIntersections(ray, eRayMask.All) ?? Enumerable.Empty<Intersection>()).ToArray();

			//KeyValuePair<Intersection, ISceneGeometry>[] intersections =
			//	aIntersections.Select(i => new KeyValuePair<Intersection, ISceneGeometry>(i, A))
			//	              .Concat(bIntersections.Select(i => new KeyValuePair<Intersection, ISceneGeometry>(i, B)))
			//	              .OrderBy(kvp => kvp.Key.RayDelta)
			//	              .ToArray();

			//int aDepth = 0;
			//int bDepth = 0;

			//for (int i = 0; i < intersections.Length; i++)
			//{
			//	(Intersection intersection, ISceneGeometry geometry) = intersections[i];
			//	float faceAmount = Vector3.Dot(ray.Direction, intersection.Normal);

			//	// Enter
			//	if (faceAmount <= 0)
			//	{
			//		if (geometry == A)
			//			aDepth++;
			//		else
			//			bDepth++;

			//		if (aDepth == bDepth)
			//			yield return intersection;
			//	}
			//	// Exit
			//	else
			//	{
			//		if (aDepth == 1 && bDepth == 1)
			//			yield return intersection;

			//		if (geometry == A)
			//			aDepth--;
			//		else
			//			bDepth--;
			//	}
			//}
		}

		protected override Aabb CalculateAabb()
		{
			if (A == null)
				return default;

			if (B == null)
				return default;

			return A.Aabb.Intersection(B.Aabb);
		}
	}
}

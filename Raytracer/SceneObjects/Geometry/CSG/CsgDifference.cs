using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Raytracer.Math;

namespace Raytracer.SceneObjects.Geometry.CSG
{
	public sealed class CsgDifference : AbstractCsg
	{
		protected override IEnumerable<Intersection> GetIntersectionsFinal(Ray ray)
		{
			Intersection[] aIntersections = (A?.GetIntersections(ray) ?? Enumerable.Empty<Intersection>()).ToArray();
			Intersection[] bIntersections = (B?.GetIntersections(ray) ?? Enumerable.Empty<Intersection>()).ToArray();

			KeyValuePair<Intersection, ISceneGeometry>[] intersections =
				aIntersections.Select(i => new KeyValuePair<Intersection, ISceneGeometry>(i, A))
				              .Concat(bIntersections.Select(i => new KeyValuePair<Intersection, ISceneGeometry>(i, B)))
				              .OrderBy(kvp => kvp.Key.RayDelta)
				              .ToArray();

			int aDepth = 0;
			int bDepth = 0;

			for (int i = 0; i < intersections.Length; i++)
			{
				(Intersection intersection, ISceneGeometry geometry) = intersections[i];
				bool subtract = geometry == B;
				float faceAmount = Vector3.Dot(ray.Direction, intersection.Normal);

				if (subtract)
				{
					// Enter
					if (faceAmount <= 0)
					{
						if (bDepth == 0 && aDepth > 0)
							yield return intersection.Flip();
						bDepth++;
					}
					// Leave
					else
					{
						bDepth--;
						if (bDepth == 0 && aDepth > 0)
							yield return intersection.Flip();
					}
				}
				else
				{
					// Enter
					if (faceAmount <= 0)
					{
						if (aDepth == 0 && bDepth <= 0)
							yield return intersection;
						aDepth++;
					}
					// Leave
					else
					{
						aDepth--;
						if (aDepth == 0 && bDepth <= 0)
							yield return intersection;
					}
				}
			}
		}

		protected override Aabb CalculateAabb()
		{
			if (A == null)
				return default;

			if (B == null)
				return A.Aabb;

			return A.Aabb - B.Aabb;
		}
	}
}

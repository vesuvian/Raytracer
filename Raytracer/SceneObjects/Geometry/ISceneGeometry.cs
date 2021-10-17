using System.Collections.Generic;
using Raytracer.Geometry;
using Raytracer.Math;

namespace Raytracer.SceneObjects.Geometry
{
	public interface ISceneGeometry : ISceneObject
	{
        eRayMask RayMask { get; }

		float SurfaceArea { get; }

		Aabb Aabb { get; }

		IEnumerable<Intersection> GetIntersections(Ray ray, eRayMask mask, float minDelta = float.NegativeInfinity,
                                                   float maxDelta = float.PositiveInfinity);
    }
}

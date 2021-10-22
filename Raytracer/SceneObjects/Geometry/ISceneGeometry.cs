using Raytracer.Geometry;
using Raytracer.Math;

namespace Raytracer.SceneObjects.Geometry
{
	public interface ISceneGeometry : ISceneObject
	{
        eRayMask RayMask { get; }

		float SurfaceArea { get; }

		Aabb Aabb { get; }

		bool GetIntersection(Ray ray, eRayMask mask, out Intersection intersection,
		                     float minDelta = float.NegativeInfinity,
		                     float maxDelta = float.PositiveInfinity);
    }
}

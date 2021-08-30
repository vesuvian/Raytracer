using System.Collections.Generic;
using Raytracer.Materials;
using Raytracer.Math;

namespace Raytracer.SceneObjects.Geometry
{
	public interface ISceneGeometry : ISceneObject
	{
		IMaterial Material { get; set; }

		eRayMask RayMask { get; set; }

		float SurfaceArea { get; }

		Aabb Aabb { get; }

		IEnumerable<Intersection> GetIntersections(Ray ray);
	}
}

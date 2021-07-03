using System.Collections.Generic;
using Raytracer.Materials;
using Raytracer.Math;

namespace Raytracer.SceneObjects.Geometry
{
	public interface ISceneGeometry : ISceneObject
	{
		IMaterial Material { get; set; }

		eRayMask RayMask { get; set; }

		IEnumerable<Intersection> GetIntersections(Ray ray);
	}
}

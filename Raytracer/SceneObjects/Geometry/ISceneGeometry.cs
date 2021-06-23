using System.Collections.Generic;
using Raytracer.Math;

namespace Raytracer.SceneObjects.Geometry
{
	public interface ISceneGeometry : ISceneObject
	{
		eRayMask RayMask { get; set; }

		IEnumerable<Intersection> GetIntersections(Ray ray);
	}
}

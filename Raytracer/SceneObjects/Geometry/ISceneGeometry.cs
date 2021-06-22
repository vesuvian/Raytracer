using System.Collections.Generic;
using Raytracer.Math;

namespace Raytracer.SceneObjects.Geometry
{
	public interface ISceneGeometry : ISceneObject
	{
		IEnumerable<Intersection> GetIntersections(Ray ray);
	}
}

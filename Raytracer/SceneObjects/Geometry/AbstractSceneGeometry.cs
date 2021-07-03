using System.Collections.Generic;
using Raytracer.Materials;
using Raytracer.Math;

namespace Raytracer.SceneObjects.Geometry
{
	public abstract class AbstractSceneGeometry : AbstractSceneObject, ISceneGeometry
	{
		public IMaterial Material { get; set; } = new Material();

		public eRayMask RayMask { get; set; } = eRayMask.All;

		public abstract IEnumerable<Intersection> GetIntersections(Ray ray);
	}
}

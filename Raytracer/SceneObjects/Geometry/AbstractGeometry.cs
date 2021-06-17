using Raytracer.Math;

namespace Raytracer.SceneObjects.Geometry
{
	public abstract class AbstractGeometry : AbstractSceneObject, IGeometry
	{
		public abstract bool GetIntersection(Ray ray, out Intersection intersection);
	}
}

using Raytracer.Math;

namespace Raytracer.SceneObjects.Geometry
{
	public abstract class AbstractSceneGeometry : AbstractSceneObject, ISceneGeometry
	{
		public abstract bool GetIntersection(Ray ray, out Intersection intersection);
	}
}

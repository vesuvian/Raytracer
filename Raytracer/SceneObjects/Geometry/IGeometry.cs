using Raytracer.Math;

namespace Raytracer.SceneObjects.Geometry
{
	public interface IGeometry : ISceneObject
	{
		bool GetIntersection(Ray ray, out Intersection intersection);
	}
}

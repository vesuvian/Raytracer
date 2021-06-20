using Raytracer.Math;

namespace Raytracer.SceneObjects.Geometry
{
	public interface ISceneGeometry : ISceneObject
	{
		bool GetIntersection(Ray ray, out Intersection intersection);
	}
}

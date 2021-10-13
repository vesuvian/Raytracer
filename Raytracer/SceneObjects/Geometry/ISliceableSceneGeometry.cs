using Raytracer.Math;

namespace Raytracer.SceneObjects.Geometry
{
    public interface ISliceableSceneGeometry : ISceneGeometry
    {
        ISceneGeometry Slice(Aabb aabb);
    }
}

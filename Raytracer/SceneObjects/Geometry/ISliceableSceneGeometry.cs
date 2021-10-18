using Raytracer.Geometry;

namespace Raytracer.SceneObjects.Geometry
{
    public interface ISliceableSceneGeometry : ISceneGeometry
    {
        int Complexity { get; }

        ISliceableSceneGeometry Slice(Aabb aabb);
    }
}

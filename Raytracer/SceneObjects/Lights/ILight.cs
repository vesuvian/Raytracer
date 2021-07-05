using System.Numerics;

namespace Raytracer.SceneObjects.Lights
{
	public interface ILight : ISceneObject
	{
		Vector4 Color { get; set; }
		bool CastShadows { get; set; }

		Vector4 Sample(Scene scene, Vector3 position, Vector3 normal);
	}
}

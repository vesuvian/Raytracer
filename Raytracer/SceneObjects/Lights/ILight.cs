using System.Drawing;
using System.Numerics;

namespace Raytracer.SceneObjects.Lights
{
	public interface ILight : ISceneObject
	{
		Color Color { get; set; }
		float Intensity { get; set; }
		float Range { get; set; }
		eFalloff Falloff { get; set; }

		Color Sample(Vector3 position, Vector3 normal);
	}
}

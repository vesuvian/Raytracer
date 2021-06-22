using System.Drawing;
using System.Numerics;

namespace Raytracer.SceneObjects.Lights
{
	public abstract class AbstractLight : AbstractSceneObject, ILight
	{
		public Color Color { get; set; } = Color.White;
		public float Intensity { get; set; } = 10;
		public float Range { get; set; } = 100;
		public eFalloff Falloff { get; set; }

		public abstract Color Sample(Vector3 position, Vector3 normal);
	}
}

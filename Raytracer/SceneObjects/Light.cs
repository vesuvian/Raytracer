using System.Drawing;

namespace Raytracer.SceneObjects
{
	public sealed class Light : AbstractSceneObject
	{
		public Color Color { get; set; }
		public float Intensity { get; set; }

		public Light()
		{
			Color = Color.White;
			Intensity = 10;
		}
	}
}

using System.Drawing;

namespace Raytracer.Layers
{
	public interface ILayer
	{
		void Render(Scene scene, Bitmap buffer);
	}
}

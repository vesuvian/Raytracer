using System.Drawing;
using Raytracer.Math;

namespace Raytracer.Layers
{
	public abstract class AbstractLayer : ILayer
	{
		public void Render(Scene scene, Bitmap buffer)
		{
			for (int y = 0; y < buffer.Height; y++)
			{
				for (int x = 0; x < buffer.Width; x++)
				{
					float xViewport = (x + 0.5f) / buffer.Width;
					float yViewport = (y + 0.5f) / buffer.Height;

					Ray ray = scene.Camera.CreateRay(xViewport, yViewport);

					Color pixel = CastRay(scene, ray);
					buffer.SetPixel(x, y, pixel);
				}
			}
		}

		protected abstract Color CastRay(Scene scene, Ray ray);
	}
}

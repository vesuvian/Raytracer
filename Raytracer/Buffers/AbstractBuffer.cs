using System.Drawing;

namespace Raytracer.Buffers
{
	public abstract class AbstractBuffer : IBuffer
	{
		public abstract int Height { get; }

		public abstract int Width { get; }

		public abstract void Dispose();

		public abstract void SetPixel(int x, int y, Color color);

		public abstract Color GetPixel(int x, int y);
	}
}

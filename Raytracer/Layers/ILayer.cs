using System;
using System.Drawing;

namespace Raytracer.Layers
{
	public interface ILayer
	{
		event EventHandler OnProgressChanged;

		DateTime Start { get; }
		DateTime End { get; }

		int Progress { get; }
		int RenderSize { get; }

		void Render(Scene scene, Buffer buffer);
	}
}

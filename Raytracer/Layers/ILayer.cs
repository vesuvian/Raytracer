using System;
using System.Threading;
using Raytracer.Buffers;

namespace Raytracer.Layers
{
	public interface ILayer
	{
		event EventHandler OnProgressChanged;

		DateTime Start { get; }
		DateTime End { get; }

		ulong Progress { get; }
		ulong RenderSize { get; }

		void Render(Scene scene, IBuffer buffer, CancellationToken cancellationToken = default);
	}
}

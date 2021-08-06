using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Raytracer.Buffers;

namespace Raytracer.Wpf
{
	public sealed class WriteableBitmapBuffer : AbstractBuffer
	{
		private readonly WriteableBitmap m_Bitmap;
		private readonly Queue<Tuple<int, int, Color>> m_Queue;

		public override int Height { get; }
		public override int Width { get; }

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="bitmap"></param>
		public WriteableBitmapBuffer(WriteableBitmap bitmap)
		{
			m_Queue = new Queue<Tuple<int, int, Color>>();

			m_Bitmap = bitmap;

			Height = m_Bitmap.PixelHeight;
			Width = m_Bitmap.PixelWidth;
		}

		public override void Dispose()
		{
		}

		public override void SetPixel(int x, int y, Color color)
		{
			lock (m_Queue)
				m_Queue.Enqueue(new Tuple<int, int, Color>(x, y, color));

			Application.Current?.Dispatcher.Invoke(DispatcherPriority.Render, (Action)Worker);
		}

		public override Color GetPixel(int x, int y)
		{
			throw new NotSupportedException();
		}

		private void Worker()
		{
			if (!m_Bitmap.TryLock(TimeSpan.Zero))
				return;

			try
			{
				Tuple<int, int, Color> tuple;
				while (TryDequeue(out tuple))
				{
					int x = tuple.Item1;
					int y = tuple.Item2;
					Color color = tuple.Item3;

					unsafe
					{
						// Get a pointer to the back buffer.
						IntPtr pBackBuffer = m_Bitmap.BackBuffer;

						// Find the address of the pixel to draw.
						pBackBuffer += y * m_Bitmap.BackBufferStride;
						pBackBuffer += x * 4;

						// Compute the pixel's color.
						int colorData = color.R << 16;
						colorData |= color.G << 8;
						colorData |= color.B;

						// Assign the color data to the pixel.
						*(int*)pBackBuffer = colorData;
					}

					m_Bitmap.AddDirtyRect(new Int32Rect(x, y, 1, 1));
				}
			}
			finally
			{
				m_Bitmap.Unlock();
			}
		}

		private bool TryDequeue(out Tuple<int, int, Color> tuple)
		{
			lock (m_Queue)
				return m_Queue.TryDequeue(out tuple);
		}
	}
}

using System;
using System.Drawing;
using System.Threading;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Raytracer.Buffers;
using Raytracer.Collections;

namespace Raytracer.Wpf
{
	public sealed class WriteableBitmapBuffer : AbstractBuffer
	{
		private readonly WriteableBitmap m_Bitmap;
		private readonly OrderedDictionary<Tuple<int, int>, Color> m_Queue;
		private readonly Color[] m_Cache;
		private readonly SemaphoreSlim m_WorkerSemaphore;

		public override int Height { get; }
		public override int Width { get; }

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="bitmap"></param>
		public WriteableBitmapBuffer(WriteableBitmap bitmap)
		{
			m_Queue = new OrderedDictionary<Tuple<int, int>, Color>();
			m_Bitmap = bitmap;
			m_Cache = new Color[m_Bitmap.PixelWidth * m_Bitmap.PixelHeight];
			m_WorkerSemaphore = new SemaphoreSlim(1, 1);

			Height = m_Bitmap.PixelHeight;
			Width = m_Bitmap.PixelWidth;
		}

		public override void Dispose()
		{
		}

		public override void SetPixel(int x, int y, Color color)
		{
			lock (m_Queue)
			{
				int index = x + Width * y;
				if (color == m_Cache[index])
					return;

				m_Cache[index] = color;
				m_Queue[new Tuple<int, int>(x, y)] = color;
			}

			Application.Current?.Dispatcher.Invoke(DispatcherPriority.Render, (Action)Worker);
		}

		public override Color GetPixel(int x, int y)
		{
			lock (m_Queue)
				return m_Cache[x + Width * y];
		}

		private void Worker()
		{
			if (!m_WorkerSemaphore.Wait(0))
				return;

			try
			{
				Tuple<int, int, Color> tuple;
				while (TryDequeue(out tuple))
				{
					int x = tuple.Item1;
					int y = tuple.Item2;
					Color color = tuple.Item3;

					m_Bitmap.Lock();

					try
					{
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
					finally
					{
						m_Bitmap.Unlock();
					}
				}
			}
			finally
			{
				m_WorkerSemaphore.Release();
			}
		}

		private bool TryDequeue(out Tuple<int, int, Color> tuple)
		{
			tuple = default;

			lock (m_Queue)
			{
				if (m_Queue.Count == 0)
					return false;

				((int x, int y), Color color) = m_Queue.Get(0);
				tuple = new Tuple<int, int, Color>(x, y, color);

				return true;
			}
		}
	}
}

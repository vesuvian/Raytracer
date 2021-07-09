using System;
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

		public override int Height { get; }
		public override int Width { get; }

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="bitmap"></param>
		public WriteableBitmapBuffer(WriteableBitmap bitmap)
		{
			m_Bitmap = bitmap;

			Height = m_Bitmap.PixelHeight;
			Width = m_Bitmap.PixelWidth;
		}

		public override void Dispose()
		{
		}

		public override void SetPixel(int x, int y, Color color)
		{
			Application.Current
			           ?.Dispatcher
			           .Invoke(DispatcherPriority.Render,
			                   (Action)delegate
			                   {
				                   try
				                   {
					                   // Reserve the back buffer for updates.
					                   m_Bitmap.Lock();

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

					                   // Specify the area of the bitmap that changed.
					                   m_Bitmap.AddDirtyRect(new Int32Rect(x, y, 1, 1));
				                   }
				                   finally
				                   {
					                   // Release the back buffer and make it available for display.
					                   m_Bitmap.Unlock();
				                   }
			                   });
		}

		public override Color GetPixel(int x, int y)
		{
			throw new System.NotImplementedException();
		}
	}
}

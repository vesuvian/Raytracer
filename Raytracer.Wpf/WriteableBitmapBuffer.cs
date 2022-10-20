using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
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
        private readonly Thread m_WorkerThread;
        private readonly CancellationTokenSource m_WorkerCancellationTokenSource;
        private readonly IntPtr m_BackBuffer;
        private readonly int m_BackBufferStride;

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
            m_BackBuffer = bitmap.BackBuffer;
            m_BackBufferStride = bitmap.BackBufferStride;

            Height = m_Bitmap.PixelHeight;
            Width = m_Bitmap.PixelWidth;

            m_WorkerCancellationTokenSource = new CancellationTokenSource();
            m_WorkerThread = new Thread(() => Worker(m_WorkerCancellationTokenSource.Token))
            {
                Name = nameof(WriteableBitmapBuffer)
            };
            m_WorkerThread.Start();
        }

        public override void Dispose()
        {
            m_WorkerCancellationTokenSource.Cancel();
            m_WorkerCancellationTokenSource.Dispose();
        }

        public override void SetPixel(int x, int y, Color color)
        {
            lock (m_Queue)
                m_Queue.Enqueue(new Tuple<int, int, Color>(x, y, color));
        }

        public override Color GetPixel(int x, int y)
        {
            unsafe
            {
                // Get a pointer to the back buffer.
                IntPtr pBackBuffer = m_BackBuffer;

                // Find the address of the pixel to read.
                pBackBuffer += y * m_BackBufferStride;
                pBackBuffer += x * 4;

                // Compute the pixel's color.
                int colorData = *(int*)pBackBuffer;

                return Color.FromArgb(colorData >> 16, colorData >> 8 & 0xFF, colorData & 0xFF);
            }
        }

        private void Worker(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                int minX = Width;
                int minY = Height;
                int maxX = 0;
                int maxY = 0;

                Tuple<int, int, Color> tuple;
                while (TryDequeue(out tuple))
                {
                    int x = tuple.Item1;
                    int y = tuple.Item2;

                    Color color = tuple.Item3;
                    if (color == GetPixel(x, y))
                        continue;

                    minX = System.Math.Min(minX, x);
                    minY = System.Math.Min(minY, y);
                    maxX = System.Math.Max(maxX, x);
                    maxY = System.Math.Max(maxY, y);

                    unsafe
                    {
                        // Get a pointer to the back buffer.
                        IntPtr pBackBuffer = m_BackBuffer;

                        // Find the address of the pixel to draw.
                        pBackBuffer += y * m_BackBufferStride;
                        pBackBuffer += x * 4;

                        // Compute the pixel's color.
                        int colorData = color.R << 16;
                        colorData |= color.G << 8;
                        colorData |= color.B;

                        // Assign the color data to the pixel.
                        *(int*)pBackBuffer = colorData;
                    }
                }

                if (maxX > 0)
                    Blit(minX, minY, maxX, maxY);
            }
        }

        private void Blit(int minX, int minY, int maxX, int maxY)
        {
            int dirtyWidth = (maxX - minX) + 1;
            int dirtyHeight = (maxY - minY) + 1;

            Application.Current?.Dispatcher.Invoke(DispatcherPriority.Render, () =>
            {
                m_Bitmap.Lock();

                try
                {
                    m_Bitmap.AddDirtyRect(new Int32Rect(minX, minY, dirtyWidth, dirtyHeight));
                }
                finally
                {
                    m_Bitmap.Unlock();
                }
            });
        }

        private bool TryDequeue(out Tuple<int, int, Color> tuple)
        {
            lock (m_Queue)
                return m_Queue.TryDequeue(out tuple);
        }
    }
}

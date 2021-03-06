using System.Drawing;
using System.Numerics;
using Raytracer.Buffers;
using Raytracer.Extensions;
using Raytracer.Utils;

namespace Raytracer.Materials.Textures
{
	public sealed class BitmapTexture : AbstractTexture
	{
		private readonly BitmapBuffer m_Buffer;

		private BitmapTexture(BitmapBuffer buffer)
		{
			m_Buffer = buffer;
		}

		public override Vector3 Sample(float u, float v)
		{
			float x = MathUtils.ModPositive(u * m_Buffer.Width, m_Buffer.Width);
			float y = MathUtils.ModPositive((1 - v) * m_Buffer.Height, m_Buffer.Height);
			return m_Buffer.GetPixelBilinear(x, y).ToRgb();
		}

		public static BitmapTexture FromPath(string path)
		{
			Image image = Image.FromFile(path);
			Bitmap bitmap = new Bitmap(image);
			BitmapBuffer buffer = BitmapBuffer.FromBitmap(bitmap);

			return new BitmapTexture(buffer);
		}
	}
}

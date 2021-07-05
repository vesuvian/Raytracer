using System.Drawing;
using System.Numerics;
using Raytracer.Utils;

namespace Raytracer.Materials.Textures
{
	public sealed class BitmapTexture : AbstractTexture
	{
		private readonly Buffer m_Buffer;

		private BitmapTexture(Buffer buffer)
		{
			m_Buffer = buffer;
		}

		public override Vector4 Sample(float u, float v)
		{
			float x = MathUtils.ModPositive(u * m_Buffer.Width, m_Buffer.Width);
			float y = MathUtils.ModPositive((1 - v) * m_Buffer.Height, m_Buffer.Height);
			return ColorUtils.ToVectorRgba(m_Buffer.GetPixelBilinear(x, y));
		}

		public static BitmapTexture FromPath(string path)
		{
			Image image = Image.FromFile(path);
			Bitmap bitmap = new Bitmap(image);
			Buffer buffer = Buffer.FromBitmap(bitmap);

			return new BitmapTexture(buffer);
		}
	}
}

using System.Drawing;
using Raytracer.Extensions;
using Raytracer.Utils;

namespace Raytracer.Materials.Textures
{
	public sealed class BitmapTexture : AbstractTexture
	{
		private readonly Bitmap m_Bitmap;

		private BitmapTexture(Bitmap bitmap)
		{
			m_Bitmap = bitmap;
		}

		public override Color Sample(float u, float v)
		{
			lock (m_Bitmap)
			{
				float x = MathUtils.ModPositive(u * m_Bitmap.Width, m_Bitmap.Width);
				float y = MathUtils.ModPositive((1 - v) * m_Bitmap.Height, m_Bitmap.Height);
				return m_Bitmap.GetPixelBilinear(x, y);
			}
		}

		public static BitmapTexture FromPath(string path)
		{
			Image checkerboardImage = Image.FromFile(path);
			Bitmap checkerboardBitmap = new Bitmap(checkerboardImage);
			return new BitmapTexture(checkerboardBitmap);
		}
	}
}

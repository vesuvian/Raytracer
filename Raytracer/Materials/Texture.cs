using System.Drawing;
using Raytracer.Extensions;
using Raytracer.Utils;

namespace Raytracer.Materials
{
	public sealed class Texture
	{
		private readonly Bitmap m_Bitmap;

		private Texture(Bitmap bitmap)
		{
			m_Bitmap = bitmap;
		}

		public Color Sample(float u, float v)
		{
			lock (m_Bitmap)
			{
				float x = MathUtils.ModPositive(u * m_Bitmap.Width, m_Bitmap.Width);
				float y = MathUtils.ModPositive((1 - v) * m_Bitmap.Height, m_Bitmap.Height);
				return m_Bitmap.GetPixelBilinear(x, y);
			}
		}

		public static Texture FromPath(string path)
		{
			Image checkerboardImage = Image.FromFile(path);
			Bitmap checkerboardBitmap = new Bitmap(checkerboardImage);
			return new Texture(checkerboardBitmap);
		}
	}
}

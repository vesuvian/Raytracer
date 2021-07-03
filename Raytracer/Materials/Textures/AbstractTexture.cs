using System.Drawing;

namespace Raytracer.Materials.Textures
{
	public abstract class AbstractTexture : ITexture
	{
		public abstract Color Sample(float u, float v);
	}
}

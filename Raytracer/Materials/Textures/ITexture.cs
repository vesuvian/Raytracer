using System.Drawing;

namespace Raytracer.Materials.Textures
{
	public interface ITexture
	{
		Color Sample(float u, float v);
	}
}
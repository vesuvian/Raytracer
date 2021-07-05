using System.Numerics;

namespace Raytracer.Materials.Textures
{
	public interface ITexture
	{
		Vector4 Sample(float u, float v);
	}
}
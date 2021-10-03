using System.Numerics;

namespace Raytracer.Materials.Textures
{
	public interface ITexture
	{
		Vector3 Sample(float u, float v);
	}
}
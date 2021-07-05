using System.Numerics;

namespace Raytracer.Materials.Textures
{
	public abstract class AbstractTexture : ITexture
	{
		public abstract Vector4 Sample(float u, float v);
	}
}

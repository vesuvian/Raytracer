using System.Drawing;
using System.Numerics;
using Raytracer.Utils;

namespace Raytracer.SceneObjects.Lights
{
	public sealed class AmbientLight : AbstractLight
	{
		public override Color Sample(Scene scene, Vector3 position, Vector3 normal)
		{
			return ColorUtils.Multiply(Color, Intensity);
		}
	}
}

using System.Drawing;
using System.Linq;
using System.Numerics;
using Raytracer.Math;
using Raytracer.Utils;

namespace Raytracer.SceneObjects.Lights
{
	public sealed class DirectionalLight : AbstractLight
	{
		public override Color Sample(Vector3 position, Vector3 normal)
		{
			float faceAmount = Vector3.Dot(normal, Forward);
			faceAmount = MathUtils.Clamp(faceAmount, 0, 1);

			return ColorUtils.Multiply(Color, faceAmount * Intensity);
		}

		public override bool CanSee(Scene scene, Vector3 position)
		{
			Ray toLight =
				new Ray
				{
					Origin = position,
					Direction = Forward * -1
				};

			return !CastShadows || !scene.GetIntersections(toLight).Any();
		}
	}
}

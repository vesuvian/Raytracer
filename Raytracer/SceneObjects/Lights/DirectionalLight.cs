using System.Drawing;
using System.Linq;
using System.Numerics;
using Raytracer.Math;
using Raytracer.Utils;

namespace Raytracer.SceneObjects.Lights
{
	public sealed class DirectionalLight : AbstractLight
	{
		public override Color Sample(Scene scene, Vector3 position, Vector3 normal)
		{
			Ray toLight =
				new Ray
				{
					Origin = position,
					Direction = Forward * -1
				};

			bool canSee =
				!CastShadows ||
				!scene.GetIntersections(toLight, eRayMask.CastShadows)
				      .Any(kvp => kvp.Value.Distance > SELF_SHADOW_TOLERANCE);

			if (!canSee)
				return Color.Black;

			float faceAmount = Vector3.Dot(normal, Forward);
			faceAmount = MathUtils.Clamp(faceAmount, 0, 1);

			return ColorUtils.Multiply(Color, faceAmount * Intensity);
		}
	}
}

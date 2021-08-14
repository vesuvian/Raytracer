using System;
using System.Linq;
using System.Numerics;
using Raytracer.Math;
using Raytracer.Utils;

namespace Raytracer.SceneObjects.Lights
{
	public sealed class DirectionalLight : AbstractLight
	{
		public override Vector4 Sample(Scene scene, Vector3 position, Vector3 normal, Random random)
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
				return ColorUtils.RgbaBlack;

			float faceAmount = Vector3.Dot(normal, Forward);
			faceAmount = MathUtils.Clamp(faceAmount, 0, 1);

			return ColorUtils.Multiply(Color, faceAmount);
		}
	}
}

using System;
using System.Numerics;
using Raytracer.Math;
using Raytracer.Utils;

namespace Raytracer.SceneObjects.Lights
{
	public sealed class DirectionalLight : AbstractLight
	{
		public float Distance { get; set; } = 1000;

		public override Vector3 Sample(Scene scene, Vector3 position, Vector3 normal, Random random)
		{
			Ray ray = new Ray(position - (Forward * Distance), Forward);

			float faceAmount = MathF.Abs(Vector3.Dot(normal, Forward));
			faceAmount = MathUtils.Clamp(faceAmount, 0, 1);

			Vector3 sample = Color * faceAmount;
			return Shadow(scene, ray, Distance, sample);
		}
	}
}

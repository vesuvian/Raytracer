using System;
using System.Numerics;
using Raytracer.Extensions;
using Raytracer.Math;
using Raytracer.Utils;

namespace Raytracer.SceneObjects
{
	public sealed class Camera : AbstractSceneObject
	{
		public float NearPlane { get; set; } = 0.001f;

		public float FarPlane { get; set; } = 1000.0f;

		public float Fov { get; set; } = 100.0f;

		public float Aspect { get; set; } = 2.0f;

		public float FocalLength { get; set; } = 10.0f;

		public float ApertureSize { get; set; } = 0.5f;

		public Matrix4x4 Projection
		{
			get
			{
				return Matrix4x4.CreatePerspectiveFieldOfView(MathUtils.DEG2RAD * Fov, Aspect, NearPlane, FarPlane);
			}
		}

		/// <summary>
		/// Creates a camera ray for the given viewport co-ordinates in the range 0 - 1 (bottom left to top right).
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		public Ray CreateRay(float x, float y)
		{
			// Calculate the local viewport ray
			float scale = (float)System.Math.Tan(MathUtils.DEG2RAD * Fov * 0.5f);
			float rayX = (2 * x - 1) * Aspect * scale;
			float rayY = (1 - 2 * y) * scale;
			Vector3 direction = Vector3.Normalize(new Vector3(rayX, rayY, -1));

			// Find the focal point
			Vector3 focalpoint = new Ray {Direction = direction}.PositionAtDelta(FocalLength);

			// Offset the start position by a random amount for depth of field
			int seed = HashCode.Combine(x, y);
			Random random = new Random(seed);
			Vector3 apertureOffset =
				new Vector3(random.NextFloat(-0.5f, 0.5f),
				            random.NextFloat(-0.5f, 0.5f),
				            random.NextFloat(-0.5f, 0.5f)) * ApertureSize;

			// Direction is now the direction from the offset position to the focal point
			direction = Vector3.Normalize(focalpoint - apertureOffset);

			return new Ray
			{
				Origin = apertureOffset,
				Direction = direction
			}.Multiply(LocalToWorld);
		}
	}
}

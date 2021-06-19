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
			float scale = (float)System.Math.Tan(MathUtils.DEG2RAD * Fov * 0.5f);

			float rayX = (2 * x - 1) * Aspect * scale;
			float rayY = (1 - 2 * y) * scale;
			Vector3 direction = LocalToWorld.MultiplyDirection(new Vector3(rayX, rayY, -1));

			return new Ray
			{
				Origin = Position,
				Direction = Vector3.Normalize(direction)
			};
		}
	}
}

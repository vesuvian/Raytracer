using System;
using System.Numerics;
using Raytracer.Extensions;
using Raytracer.Math;
using Raytracer.Utils;

namespace Raytracer.SceneObjects.Cameras
{
	public sealed class PerspectiveCamera : AbstractCamera
	{
		private float m_Fov = 40.0f;
		private float m_Aspect = 2.0f;

		public float Fov
		{
			get { return m_Fov; }
			set
			{
				m_Fov = value;
				HandleCameraChange();
			}
		}

		public float Aspect
		{
			get { return m_Aspect; }
			set
			{
				m_Aspect = value;
				HandleCameraChange();
			}
		}

		public float FocalLength { get; set; } = 10.0f;

		public float ApertureSize { get; set; }

		/// <summary>
		/// Creates a camera ray for the given viewport co-ordinates in the range 0 - 1 (bottom left to top right).
		/// </summary>
		/// <param name="minX"></param>
		/// <param name="maxX"></param>
		/// <param name="minY"></param>
		/// <param name="maxY"></param>
		/// <param name="random"></param>
		/// <returns></returns>
		public override Ray CreateRay(float minX, float maxX, float minY, float maxY, Random random)
		{
            var x = random.NextFloat(minX, maxX);
            var y = random.NextFloat(minY, maxY);

            // Calculate the local viewport ray
			float rayX = -1 + x * 2;
			float rayY = 1 - y * 2;

			Vector3 direction = Vector3.Normalize(ProjectionInverse.MultiplyPoint(new Vector3(rayX, rayY, 0)));
			direction.Z = -direction.Z;

			// Find the focal point
			Vector3 focalpoint = direction * FocalLength;

			// Offset the start position by a random amount for depth of field
			Vector3 apertureOffset =
				new Vector3(random.NextFloat(-0.5f, 0.5f),
				            random.NextFloat(-0.5f, 0.5f),
				            random.NextFloat(-0.5f, 0.5f)) * ApertureSize;

			// Direction is now the direction from the offset position to the focal point
			Vector3 apertureOffsetDirection = Vector3.Normalize(focalpoint - apertureOffset);

			return new Ray
			{
				Origin = apertureOffset,
				Direction = apertureOffsetDirection
			}.Multiply(LocalToWorld);
		}

		protected override Matrix4x4 CalculatePerspective()
		{
			return Matrix4x4.CreatePerspectiveFieldOfView(MathUtils.DEG2RAD * Fov, Aspect, NearPlane, FarPlane);
		}
	}
}

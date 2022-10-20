using System;
using System.Numerics;
using Raytracer.Extensions;
using Raytracer.Math;

namespace Raytracer.SceneObjects.Cameras
{
	public sealed class OrthographicCamera : AbstractCamera
	{
		private float m_Width = 1.0f;
		private float m_Height = 1.0f;

		public float Width
		{
			get { return m_Width; }
			set
			{
				m_Width = value;
				HandleCameraChange();
			}
		}

		public float Height
		{
			get { return m_Height; }
			set
			{
				m_Height = value;
				HandleCameraChange();
			}
		}

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

			Vector3 start = ProjectionInverse.MultiplyPoint(new Vector3(rayX, rayY, 0));
			Vector3 end = ProjectionInverse.MultiplyPoint(new Vector3(rayX, rayY, -1));
			Vector3 direction = Vector3.Normalize(end - start);

			return new Ray(start, direction).Multiply(LocalToWorld);
		}

		protected override Matrix4x4 CalculatePerspective()
		{
			return Matrix4x4.CreateOrthographic(Width, Height, NearPlane, FarPlane);
		}
	}
}

using System;
using System.Numerics;
using Raytracer.Extensions;
using Raytracer.Math;

namespace Raytracer.SceneObjects.Cameras
{
	public interface ICamera : ISceneObject
	{
		int Samples { get; set; }

		float NearPlane { get; }

		float FarPlane { get; }

		Matrix4x4 Projection { get; }

		Matrix4x4 ProjectionInverse { get; }

		/// <summary>
		/// Creates a camera ray for the given viewport co-ordinates in the range 0 - 1 (bottom left to top right).
		/// </summary>
		/// <param name="minX"></param>
		/// <param name="maxX"></param>
		/// <param name="minY"></param>
		/// <param name="maxY"></param>
		/// <param name="random"></param>
		/// <returns></returns>
		Ray CreateRay(float minX, float maxX, float minY, float maxY, Random random);
	}

	public static class CameraExtensions
	{
		public static Vector3 WorldToEye(this ICamera extends, Vector3 world)
		{
			return extends.WorldToLocal.MultiplyPoint(world);
		}

		public static Vector4 EyeToHomogenousClipSpace(this ICamera extends, Vector3 eye)
		{
			return extends.Projection.MultiplyPoint(eye.ToVector4(-eye.Z));
		}

		public static Vector3 HomogenousClipSpaceToNormalizedDeviceSpace(
			this ICamera extends, Vector4 homogenousClipSpace)
		{
			return new Vector3(homogenousClipSpace.X / homogenousClipSpace.W,
			                   homogenousClipSpace.Y / homogenousClipSpace.W,
			                   homogenousClipSpace.Z / homogenousClipSpace.W);
		}

		public static Vector3 WorldToNormalizedDeviceSpace(this ICamera extends, Vector3 world)
		{
			Vector3 eye = extends.WorldToEye(world);
			Vector4 homogenousClipSpace = extends.EyeToHomogenousClipSpace(eye);
			return extends.HomogenousClipSpaceToNormalizedDeviceSpace(homogenousClipSpace);
		}

		public static Vector2 NormalizedDeviceSpaceToViewport(this ICamera extends, Vector3 normalizedDeviceSpace, int width, int height)
		{
			Vector2 normalized =
				new Vector2(1 - (normalizedDeviceSpace.X + 1) / 2,
				            (normalizedDeviceSpace.Y + 1) / 2);

			return new Vector2(width, height) * normalized;
		}
	}
}

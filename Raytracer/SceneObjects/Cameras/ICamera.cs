using System;
using System.Numerics;
using Raytracer.Math;

namespace Raytracer.SceneObjects.Cameras
{
	public interface ICamera : ISceneObject
	{
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
}

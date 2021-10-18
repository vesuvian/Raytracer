using System;
using System.Drawing;
using System.Numerics;
using Raytracer.Buffers;
using Raytracer.Extensions;
using Raytracer.Geometry;
using Raytracer.SceneObjects.Cameras;

namespace Raytracer.Utils
{
	public sealed class DrawingUtils
	{
		private static Aabb s_NormalizedDeviceSpace =
			new Aabb { Min = new Vector3(-1, -1, -1), Max = new Vector3(1, 1, 1) };

		public static void DrawAabb(ICamera camera, Aabb aabb, IBuffer buffer, Color color)
		{
			// Bottom square
			DrawLine(camera, new Vector3(aabb.Min.X, aabb.Min.Y, aabb.Min.Z), new Vector3(aabb.Max.X, aabb.Min.Y, aabb.Min.Z), buffer, color);
			DrawLine(camera, new Vector3(aabb.Min.X, aabb.Min.Y, aabb.Max.Z), new Vector3(aabb.Max.X, aabb.Min.Y, aabb.Max.Z), buffer, color);
			DrawLine(camera, new Vector3(aabb.Min.X, aabb.Min.Y, aabb.Min.Z), new Vector3(aabb.Min.X, aabb.Min.Y, aabb.Max.Z), buffer, color);
			DrawLine(camera, new Vector3(aabb.Max.X, aabb.Min.Y, aabb.Min.Z), new Vector3(aabb.Max.X, aabb.Min.Y, aabb.Max.Z), buffer, color);

			// Top square
			DrawLine(camera, new Vector3(aabb.Min.X, aabb.Max.Y, aabb.Min.Z), new Vector3(aabb.Max.X, aabb.Max.Y, aabb.Min.Z), buffer, color);
			DrawLine(camera, new Vector3(aabb.Min.X, aabb.Max.Y, aabb.Max.Z), new Vector3(aabb.Max.X, aabb.Max.Y, aabb.Max.Z), buffer, color);
			DrawLine(camera, new Vector3(aabb.Min.X, aabb.Max.Y, aabb.Min.Z), new Vector3(aabb.Min.X, aabb.Max.Y, aabb.Max.Z), buffer, color);
			DrawLine(camera, new Vector3(aabb.Max.X, aabb.Max.Y, aabb.Min.Z), new Vector3(aabb.Max.X, aabb.Max.Y, aabb.Max.Z), buffer, color);

			// Struts
			DrawLine(camera, new Vector3(aabb.Min.X, aabb.Min.Y, aabb.Min.Z), new Vector3(aabb.Min.X, aabb.Max.Y, aabb.Min.Z), buffer, color);
			DrawLine(camera, new Vector3(aabb.Max.X, aabb.Min.Y, aabb.Min.Z), new Vector3(aabb.Max.X, aabb.Max.Y, aabb.Min.Z), buffer, color);
			DrawLine(camera, new Vector3(aabb.Min.X, aabb.Min.Y, aabb.Max.Z), new Vector3(aabb.Min.X, aabb.Max.Y, aabb.Max.Z), buffer, color);
			DrawLine(camera, new Vector3(aabb.Max.X, aabb.Min.Y, aabb.Max.Z), new Vector3(aabb.Max.X, aabb.Max.Y, aabb.Max.Z), buffer, color);
		}

		public static void DrawLine(ICamera camera, Vector3 a, Vector3 b, IBuffer buffer, Color color)
		{
			Vector3 ndcA = camera.WorldToNormalizedDeviceSpace(a);
			Vector3 ndcB = camera.WorldToNormalizedDeviceSpace(b);

			// Clip to the camera frustum
			Vector3 clippedA;
			Vector3 clippedB;
			if (!s_NormalizedDeviceSpace.ClipLine(ndcA, ndcB, out clippedA, out clippedB))
				return;

			Vector2 bufferA = camera.NormalizedDeviceSpaceToViewport(clippedA, buffer.Width, buffer.Height);
			Vector2 bufferB = camera.NormalizedDeviceSpaceToViewport(clippedB, buffer.Width, buffer.Height);

			DrawLine(bufferA, bufferB, buffer, color);
		}

		public static void DrawLine(Vector2 start, Vector2 end, IBuffer buffer, Color color)
		{
			Aabb rect = GetBufferBounds(buffer);

			// Clip to the buffer
			Vector3 clippedA;
			Vector3 clippedB;
			if (!rect.ClipLine(start.ToVector3(), end.ToVector3(), out clippedA, out clippedB))
				return;

			start = clippedA.ToVector2();
			end = clippedB.ToVector2();

			var difX = end.X - start.X;
			var difY = end.Y - start.Y;
			var dist = MathF.Abs(difX) + MathF.Abs(difY);

			var dx = difX / dist;
			var dy = difY / dist;

			for (var i = 0; i <= MathF.Ceiling(dist); i++)
			{
				int x = (int)(start.X + dx * i);
				int y = (int)(start.Y + dy * i);

				if (x >= 0 && x < buffer.Width && y >= 0 && y < buffer.Height)
					buffer.SetPixel(x, y, color);
			}
		}

		private static Aabb GetBufferBounds(IBuffer buffer)
		{
			return new Aabb
			{
				Min = new Vector3(0, 0, float.NegativeInfinity),
				Max = new Vector3(buffer.Width, buffer.Height, float.PositiveInfinity)
			};
		}
	}
}

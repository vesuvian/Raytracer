using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Numerics;
using Raytracer.Extensions;
using Raytracer.Math;
using Raytracer.SceneObjects;
using Raytracer.SceneObjects.Geometry;
using Raytracer.Utils;
using Plane = Raytracer.SceneObjects.Geometry.Plane;

namespace Raytracer
{
	public static class Program
	{
		private const int WIDTH = 1920;
		private const int HEIGHT = 1080;
		private const float NEAR_PLANE = 0.01f;
		private const float FAR_PLANE = 1000.0f;
		private const float FOV = 100;
		private const float RENDER_SCALE = 0.5f;

		private const string PATH = @"C:\\Temp\\raytrace.bmp";

		public static void Main()
		{
			Scene scene = new Scene
			{
				Camera = new Camera
				{
					Position = new Vector3(0, 0, -10),
					Rotation = Quaternion.CreateFromYawPitchRoll(MathUtils.DEG2RAD * 180, 0, 0),
					//Projection = Matrix4x4.CreatePerspective(WIDTH, HEIGHT, NEAR_PLANE, FAR_PLANE)
				},
				Lights = new List<Light>
				{
					new Light
					{
						Position = new Vector3(0, 1, 0)
					}
				},
				Geometry = new List<IGeometry>
				{
					new Sphere
					{
						Position = new Vector3(3, 1, 7.5f),
						Scale = new Vector3(2, 1, 1),
						Rotation = Quaternion.CreateFromYawPitchRoll(MathUtils.DEG2RAD * 45, MathUtils.DEG2RAD * 15, MathUtils.DEG2RAD * 30),
						Radius = 2
					},
					new Plane
					{
						Position = new Vector3(0, 0, 0),
						Rotation = Quaternion.CreateFromYawPitchRoll(0, MathUtils.DEG2RAD * 45, 0)
					}
				}
			};

			using (Bitmap buffer = new Bitmap((int)(WIDTH * RENDER_SCALE), (int)(HEIGHT * RENDER_SCALE)))
			{
				Render(scene, buffer);

				if (File.Exists(PATH))
					File.Delete(PATH);

				using (Bitmap output = new Bitmap(WIDTH, HEIGHT))
				{
					using (Graphics graphics = Graphics.FromImage(output))
						graphics.DrawImage(buffer, 0, 0, WIDTH, HEIGHT);

					output.Save(PATH, ImageFormat.Bmp);
				}
			}

			Process.Start("cmd.exe", $"/c {PATH}");
		}

		public static void Render(Scene scene, Bitmap buffer)
		{
			float aspect = (float)buffer.Width / buffer.Height;
			float scale = (float)System.Math.Tan(MathUtils.DEG2RAD * FOV * 0.5f);

			for (int y = 0; y < buffer.Height; y++)
			{
				for (int x = 0; x < buffer.Width; x++)
				{
					float rayX = (2 * (x + 0.5f) / buffer.Width - 1) * aspect * scale;
					float rayY = (1 - 2 * (y + 0.5f) / buffer.Height) * scale;
					Vector3 direction = scene.Camera.LocalToWorld.MultiplyDirection(new Vector3(rayX, rayY, -1));

					Ray ray = new Ray
					{
						Origin = scene.Camera.Position,
						Direction = Vector3.Normalize(direction)
					};

					Color pixel = CastRay(scene, ray);
					buffer.SetPixel(x, y, pixel);
				}
			}
		}

		public static Color CastRay(Scene scene, Ray ray)
		{
			IGeometry closest = null;
			Intersection? closestIntersection = null;

			foreach (IGeometry obj in scene.Geometry)
			{
				Intersection intersection;
				if (!obj.GetIntersection(ray, out intersection))
					continue;

				if (closestIntersection != null &&
					closestIntersection.Value.Distance <= intersection.Distance)
					continue;

				closest = obj;
				closestIntersection = intersection;
			}

			if (closest == null)
				return Color.Black;

			if (closestIntersection == null)
				return Color.Red;

			float t = MathUtils.Clamp(closestIntersection.Value.Distance, NEAR_PLANE, FAR_PLANE) / (FAR_PLANE - NEAR_PLANE);
			return ColorUtils.Lerp(Color.White, Color.Black, t);
		}
	}
}

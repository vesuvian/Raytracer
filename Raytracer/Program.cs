using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Numerics;
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
		private const float RENDER_SCALE = 2.0f;

		private const string PATH = @"C:\\Temp\\raytrace.bmp";

		public static void Main()
		{
			Scene scene = new Scene
			{
				Camera = new Camera
				{
					Position = new Vector3(0, 2, -10),
					Rotation = Quaternion.CreateFromYawPitchRoll(MathUtils.DEG2RAD * 180, 0, 0),
					NearPlane = 0.01f,
					FarPlane = 20.0f,
					Fov = 100,
					Aspect = WIDTH / (float)HEIGHT
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
						Radius = 5
					},
					new Plane()
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

		private static void Render(Scene scene, Bitmap buffer)
		{
			for (int y = 0; y < buffer.Height; y++)
			{
				for (int x = 0; x < buffer.Width; x++)
				{
					float xViewport = (x + 0.5f) / buffer.Width;
					float yViewport = (y + 0.5f) / buffer.Height;

					Ray ray = scene.Camera.CreateRay(xViewport, yViewport);

					Color pixel = CastRay(scene, ray);
					buffer.SetPixel(x, y, pixel);
				}
			}
		}

		private static Color CastRay(Scene scene, Ray ray)
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

			float planarDistance = Plane.Distance(scene.Camera.Position, scene.Camera.Forward, closestIntersection.Value.Position, out _);
			float t = MathUtils.Clamp(planarDistance, scene.Camera.NearPlane, scene.Camera.FarPlane) /
			          (scene.Camera.FarPlane - scene.Camera.NearPlane);
			return ColorUtils.Lerp(Color.White, Color.Black, t);
		}
	}
}

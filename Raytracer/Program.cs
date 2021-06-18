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

namespace Raytracer
{
	public static class Program
	{
		const int WIDTH = 1920;
		const int HEIGHT = 1080;
		const float NEAR_PLANE = 0.01f;
		const float FAR_PLANE = 1000.0f;
		const float ASPECT = (float)WIDTH / HEIGHT;
		const float FOV = 100;

		const string PATH = @"C:\\Temp\\raytrace.bmp";

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
						Position = new Vector3(0, 0, 0),
						Scale = new Vector3(1, 1, 1),
						Radius = 5
					},
				}
			};

			using (Bitmap buffer = new Bitmap(WIDTH, HEIGHT))
			{
				Render(scene, buffer);

				if (File.Exists(PATH))
					File.Delete(PATH);
				buffer.Save(PATH, ImageFormat.Bmp);
			}

			Process.Start("cmd.exe", $"/c {PATH}");
		}

		public static void Render(Scene scene, Bitmap buffer)
		{
			float scale = (float)System.Math.Tan(MathUtils.DEG2RAD * FOV * 0.5f);

			for (int y = 0; y < buffer.Height; y++)
			{
				for (int x = 0; x < buffer.Width; x++)
				{
					float rayX = (2 * (x + 0.5f) / (float)WIDTH - 1) * ASPECT * scale;
					float rayY = (1 - 2 * (y + 0.5f) / (float)HEIGHT) * scale;
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

			return closest == null ? Color.Black : Color.Red;
		}
	}
}

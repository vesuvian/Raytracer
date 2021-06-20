using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Numerics;
using System.Threading.Tasks;
using Raytracer.Layers;
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

		private const string PATH = @"C:\\Temp\\Raytracer\\";

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
					new Cube
					{
						Position = new Vector3(13f, 5, 0),
						Scale = new Vector3(2, 2, 2),
						Rotation = Quaternion.CreateFromYawPitchRoll(MathUtils.DEG2RAD * 45, MathUtils.DEG2RAD * 45, MathUtils.DEG2RAD * 45)
					},
					new Sphere
					{
						Position = new Vector3(3, 1, 7.5f),
						Scale = new Vector3(2, 1, 1),
						Rotation = Quaternion.CreateFromYawPitchRoll(MathUtils.DEG2RAD * 45, MathUtils.DEG2RAD * 15, MathUtils.DEG2RAD * 30),
						Radius = 5
					},
					new Sphere
					{
						Position = new Vector3(-3, 10, 0),
						Radius = 5
					},
					new Plane(),
					new Plane
					{
						Position = new Vector3(-10, 0, 0),
						Rotation = Quaternion.CreateFromYawPitchRoll(0, 0, MathUtils.DEG2RAD * 90)
					}
				},
				Layers = new List<ILayer>
				{
					new DepthLayer(),
					new WorldNormalsLayer(),
					new ViewNormalsLayer()
				}
			};

			Parallel.ForEach(scene.Layers, layer => Render(scene, layer));
		}

		private static void Render(Scene scene, ILayer layer)
		{
			string path = Path.Combine(PATH, layer.GetType().Name + ".bmp");

			using (Bitmap buffer = new Bitmap((int)(WIDTH * RENDER_SCALE), (int)(HEIGHT * RENDER_SCALE)))
			{
				layer.Render(scene, buffer);

				Directory.CreateDirectory(PATH);
				if (File.Exists(path))
					File.Delete(path);

				using (Bitmap output = new Bitmap(WIDTH, HEIGHT))
				{
					using (Graphics graphics = Graphics.FromImage(output))
						graphics.DrawImage(buffer, 0, 0, WIDTH, HEIGHT);

					output.Save(path, ImageFormat.Bmp);
				}
			}

			Process.Start("cmd.exe", $"/c {path}");
		}
	}
}

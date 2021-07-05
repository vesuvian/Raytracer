using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Numerics;
using System.Threading.Tasks;
using Raytracer.Layers;
using Raytracer.Materials;
using Raytracer.Materials.Textures;
using Raytracer.Parsers;
using Raytracer.SceneObjects;
using Raytracer.SceneObjects.Geometry;
using Raytracer.SceneObjects.Lights;
using Raytracer.Utils;
using Plane = Raytracer.SceneObjects.Geometry.Plane;

namespace Raytracer
{
	public static class Program
	{
		private const int WIDTH = 1920;
		private const int HEIGHT = 1080;
		private const float RENDER_SCALE = 1.0f;

		private const string PATH = @"C:\\Temp\\Raytracer\\";

		public static void Main()
		{
			Console.CursorVisible = false;

			BitmapTexture normal = BitmapTexture.FromPath("Resources\\sci_fi_normal.jpg");
			
			Scene scene = new Scene
			{
				Camera = new Camera
				{
					Position = new Vector3(5, 2, -20),
					NearPlane = 0.01f,
					FarPlane = 40.0f,
					Fov = 40,
					Aspect = WIDTH / (float)HEIGHT
				},
				Lights = new List<ILight>
				{
					new PointLight
					{
						Position = new Vector3(10, 100, -5),
						Color = Color.Red,
						Range = 200,
						Intensity = 4,
						Falloff = eFalloff.Linear
					},
					new PointLight
					{
						Position = new Vector3(0, 10, 10),
						Color = Color.Green,
						Range = 40,
						Intensity = 4,
						Falloff = eFalloff.Linear
					},
					new PointLight
					{
						Position = new Vector3(0, 1, -20),
						Color = Color.White,
						Range = 80,
						Intensity = 4,
						Falloff = eFalloff.Linear
					}
				},
				Geometry = new List<ISceneGeometry>
				{
					new Sphere
					{
						Radius = 100000,
						RayMask = eRayMask.Visible,
						Material = new Material
						{
							Emission = BitmapTexture.FromPath("Resources\\skysphere.jpg")
						}
					},
					new Cube
					{
						Position = new Vector3(13f, 5, 0),
						Scale = new Vector3(2, 2, 2),
						Rotation = Quaternion.CreateFromYawPitchRoll(MathUtils.DEG2RAD * 45, MathUtils.DEG2RAD * 45, MathUtils.DEG2RAD * 45),
						Material = new Material
						{
							Diffuse = new CheckerboardTexture(),
							Normal = normal,
						}
					},
					new Sphere
					{
						Position = new Vector3(3, 1, 7.5f),
						Scale = new Vector3(2, 1, 1),
						Rotation = Quaternion.CreateFromYawPitchRoll(MathUtils.DEG2RAD * 45, MathUtils.DEG2RAD * 15, MathUtils.DEG2RAD * 30),
						Radius = 5,
						Material = new Material
						{
							Diffuse = new SolidColorTexture { Color = Color.SlateGray },
							Reflectivity = new SolidColorTexture { Color = Color.Gray },
							Roughness = new SolidColorTexture { Color = Color.Gray },
							Scale = new Vector2(1 / 3.0f, 1)
						}
					},
					new Sphere
					{
						Position = new Vector3(-3, 10, 0),
						Radius = 5,
						Material = new Material
						{
							Diffuse = new CheckerboardTexture(),
							Normal = normal,
							Scale = new Vector2(1 / 3.0f, 1)
						}
					},
					new Plane
					{
						Material = new Material
						{
							Diffuse = new CheckerboardTexture(),
							Normal = normal,
							Scale = new Vector2(5, 5)
						}
					},
					new Plane
					{
						Position = new Vector3(-10, 0, 0),
						Rotation = Quaternion.CreateFromYawPitchRoll(0, MathUtils.DEG2RAD * 90, MathUtils.DEG2RAD * 90),
						RayMask = eRayMask.Visible,
						Material = new Material
						{
							Diffuse = new SolidColorTexture { Color = Color.Orange},
							Normal = normal,
							Scale = new Vector2(5, 5)
						}
					},
					new Plane
					{
						Position = new Vector3(0, 0, 40),
						Rotation = Quaternion.CreateFromYawPitchRoll(0, MathUtils.DEG2RAD * 90, 0),
						RayMask = eRayMask.Visible,
						Material = new Material
						{
							Diffuse = new SolidColorTexture { Color = Color.Purple},
							Normal = normal,
							Scale = new Vector2(5, 5)
						}
					},
					new Model
					{
						Scale = Vector3.One * 0.2f,
						Position = new Vector3(3, 2, -5),
						Rotation = Quaternion.CreateFromYawPitchRoll(MathUtils.DEG2RAD * -45, MathUtils.DEG2RAD * -15, MathUtils.DEG2RAD * 30),
						Mesh = new ObjMeshParser().Parse("Resources\\teapot.obj"),
						Material = new Material
						{
							Diffuse = new SolidColorTexture { Color = Color.LightSlateGray },
							Normal = normal,
							Reflectivity = new SolidColorTexture { Color = Color.Gray }
						}
					}
				},
				Layers = new List<ILayer>
				{
					//new DepthLayer(),
					//new WorldNormalsLayer(),
					//new ViewNormalsLayer(),
					//new WorldPositionLayer(),
					//new LightsLayer(),
					//new UnlitLayer(),
					new FullLayer()
				}
			};

			for (int index = 0; index < scene.Layers.Count; index++)
			{
				int index1 = index;
				scene.Layers[index].OnProgressChanged += (sender, args) => PrintProgress(scene, index1);
			}

			Parallel.ForEach(scene.Layers,
			                 layer =>
			                 {
				                 Render(scene, layer);
			                 });
		}

		private static void PrintProgress(Scene scene, int layerIndex)
		{
			lock (scene)
			{
				ILayer layer = scene.Layers[layerIndex];

				char spin = (layer.Progress % 4) switch
				{
					0 => '/',
					1 => '-',
					2 => '\\',
					3 => '|',
					_ => default
				};

				TimeSpan elapsed = DateTime.UtcNow - layer.Start;
				float percent = layer.RenderSize == 0 ? 0 : (layer.Progress / (float)layer.RenderSize);

				TimeSpan remaining =
					System.Math.Abs(layer.Progress) < 0.0001f
						? TimeSpan.MaxValue
						: (elapsed / percent) * (1 - percent);

				Console.SetCursorPosition(0, layerIndex);
				Console.Write("{0} {1} - {2:P} ({3} remaining)           ", spin, layer.GetType().Name, percent, remaining);
				Console.SetCursorPosition(0, scene.Layers.Count);
			}
		}

		private static void Render(Scene scene, ILayer layer)
		{
			string path = Path.Combine(PATH, layer.GetType().Name + ".bmp");

			using (Buffer buffer = new Buffer((int)(WIDTH * RENDER_SCALE), (int)(HEIGHT * RENDER_SCALE)))
			{
				layer.Render(scene, buffer);

				Directory.CreateDirectory(PATH);
				if (File.Exists(path))
					File.Delete(path);

				using (Bitmap output = new Bitmap(WIDTH, HEIGHT))
				{
					using (Graphics graphics = Graphics.FromImage(output))
						graphics.DrawImage(buffer.Bitmap, 0, 0, WIDTH, HEIGHT);

					output.Save(path, ImageFormat.Bmp);
				}
			}

			Process.Start("cmd.exe", $"/c {path}");
		}
	}
}

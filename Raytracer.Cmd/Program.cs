﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.IO;
using System.Numerics;
using System.Threading.Tasks;
using Raytracer.Buffers;
using Raytracer.Layers;
using Raytracer.Materials;
using Raytracer.Materials.Textures;
using Raytracer.Parsers;
using Raytracer.SceneObjects;
using Raytracer.SceneObjects.Geometry;
using Raytracer.SceneObjects.Lights;
using Raytracer.Utils;
using Plane = Raytracer.SceneObjects.Geometry.Plane;

namespace Raytracer.Cmd
{
	public static class Program
	{
		private const int WIDTH = 1920;
		private const int HEIGHT = 1080;

		private const string PATH = @"C:\\Temp\\Raytracer\\";

		public static void Main()
		{
			Console.CursorVisible = false;

			BitmapTexture normal = BitmapTexture.FromPath("Resources\\TexturesCom_Wall_Stone2_3x3_1K_normal.tif");
			
			Scene scene = new Scene
			{
				Camera = new Camera
				{
					Position = new Vector3(5, 2, -20),
					NearPlane = 0.01f,
					FarPlane = 40.0f,
					Fov = 40,
					//Samples = 32,
					//FocalLength = 20,
					//ApertureSize = 0.2f,
					Aspect = WIDTH / (float)HEIGHT
				},
				Lights = new List<ILight>
				{
					new PointLight
					{
						Position = new Vector3(10, 100, -5),
						Color = new Vector4(10, 0, 0, 1),
						Range = 200,
						//SoftShadowRadius = 2,
						//Samples = 8,
						Falloff = eFalloff.Linear
					},
					new PointLight
					{
						Position = new Vector3(0, 10, 10),
						Color = new Vector4(0, 10, 0, 1),
						Range = 40,
						//SoftShadowRadius = 2,
						//Samples = 8,
						Falloff = eFalloff.Linear
					},
					new PointLight
					{
						Position = new Vector3(0, 1, -20),
						Color = new Vector4(10, 10, 10, 1),
						Range = 80,
						//SoftShadowRadius = 2,
						//Samples = 8,
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
							Diffuse = new CheckerboardTexture { ColorA = new Vector4(0.9f), ColorB = new Vector4(0.1f) },
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
							Diffuse = new SolidColorTexture { Color = new Vector4(0.5f) },
							Reflectivity = new SolidColorTexture { Color = new Vector4(0.5f) },
							Roughness = new SolidColorTexture { Color = new Vector4(0.5f) },
							Scale = new Vector2(1 / 3.0f, 1)
						}
					},
					new Sphere
					{
						Position = new Vector3(-3, 10, 0),
						Radius = 5,
						Material = new Material
						{
							Diffuse = new CheckerboardTexture { ColorA = new Vector4(0.9f), ColorB = new Vector4(0.1f) },
							Normal = normal,
							Scale = new Vector2(1 / 3.0f, 1)
						}
					},
					new Sphere
					{
						Position = new Vector3(-4, 2, 0),
						Radius = 2,
						Material = new Material
						{
							Diffuse = new CheckerboardTexture { ColorA = new Vector4(0.9f), ColorB = new Vector4(0.1f) },
							Reflectivity = new CheckerboardTexture(),
							Scale = new Vector2(1 / 3.0f, 1)
						}
					},
					new Sphere
					{
						Position = new Vector3(0, 2, 0),
						Radius = 2,
						Material = new Material
						{
							Diffuse = new CheckerboardTexture { ColorA = new Vector4(0.9f), ColorB = new Vector4(0.1f) },
							Reflectivity = new SolidColorTexture { Color = new Vector4(0.5f) },
							Scale = new Vector2(1 / 3.0f, 1)
						}
					},
					new Sphere
					{
						Position = new Vector3(4, 2, 0),
						Radius = 2,
						Material = new Material
						{
							Diffuse = new CheckerboardTexture { ColorA = new Vector4(0.9f), ColorB = new Vector4(0.1f) },
							Reflectivity = new SolidColorTexture { Color = ColorUtils.RgbaWhite },
							Scale = new Vector2(1 / 3.0f, 1)
						}
					},
					new Sphere
					{
						Position = new Vector3(8, 2, 0),
						Radius = 2,
						Material = new Material
						{
							Diffuse = new CheckerboardTexture { ColorA = new Vector4(0.9f), ColorB = new Vector4(0.1f) },
							Reflectivity = new SolidColorTexture { Color = ColorUtils.RgbaWhite },
							Roughness = new CheckerboardTexture
							{
								ColorA = new Vector4(0.5f)
							},
							Scale = new Vector2(1 / 3.0f, 1)
						}
					},
					new Sphere
					{
						Position = new Vector3(12, 2, 0),
						Radius = 2,
						Material = new Material
						{
							Diffuse = new SolidColorTexture { Color = new Vector4(0, 1, 0, 1) }
						}
					},
					new Sphere
					{
						Position = new Vector3(16, 2, 0),
						Radius = 2,
						Material = new Material
						{
							Diffuse = new SolidColorTexture { Color = new Vector4(0, 0, 1, 1) },
							Emission = new SolidColorTexture { Color = new Vector4(0, 0, 1, 1) }
						}
					},
					new Plane
					{
						Material = new Material
						{
							Diffuse = new CheckerboardTexture { ColorA = new Vector4(0.9f), ColorB = new Vector4(0.1f) },
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
							Diffuse = new SolidColorTexture { Color = new Vector4(0.5f) },
							Normal = normal,
							Reflectivity = new SolidColorTexture { Color = new Vector4(0.5f) }
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

			using (BitmapBuffer buffer = new BitmapBuffer(WIDTH, HEIGHT))
			{
				layer.Render(scene, buffer);

				Directory.CreateDirectory(PATH);
				if (File.Exists(path))
					File.Delete(path);

				buffer.Bitmap.Save(path, ImageFormat.Bmp);
			}

			Process.Start("cmd.exe", $"/c {path}");
		}
	}
}
using System;
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
using Raytracer.SceneObjects.Cameras;
using Raytracer.SceneObjects.Geometry;
using Raytracer.SceneObjects.Lights;
using Raytracer.Utils;

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
                Samples = int.MaxValue,
				GlobalIlluminationSamples = 4,
				AmbientOcclusionSamples = 16,
				AmbientOcclusionScale = 0.5f,
				Camera = new PerspectiveCamera
				{
					Position = new Vector3(5, 2, -20),
					NearPlane = 0.01f,
					FarPlane = 40.0f,
					Fov = 40,
					FocalLength = 18,
					ApertureSize = 0.2f,
					Aspect = WIDTH / (float)HEIGHT
				},
				Lights = new List<ILight>
				{
					new PointLight
					{
						Position = new Vector3(10, 100, -5),
						Color = new Vector3(20000, 0, 0),
						SoftShadowRadius = 2,
						Falloff = eFalloff.Quadratic
					},
					new PointLight
					{
						Position = new Vector3(0, 10, 10),
						Color = new Vector3(0, 500, 0),
						SoftShadowRadius = 2,
						Falloff = eFalloff.Quadratic
					},
					new PointLight
					{
						Position = new Vector3(0, 1, -20),
						Color = new Vector3(300, 300, 300),
						SoftShadowRadius = 2,
						Falloff = eFalloff.Quadratic
					}
				},
				Geometry = new List<ISceneGeometry>
				{
					new Sphere
					{
						Radius = 100000,
						RayMask = eRayMask.Visible,
						Material = new EmissiveMaterial
						{
							Emission = BitmapTexture.FromPath("Resources\\skysphere.jpg")
						}
					},
					new Cylinder
					{
						Position = new Vector3(5, 2, -10),
						Height = 4,
						RayMask = eRayMask.Visible | eRayMask.CastShadows,
						Material = new RefractiveMaterial
						{
							Color = new Vector3(1, 0, 0)
						}
					},
					new Cube
					{
						Position = new Vector3(13f, 5, 0),
						Scale = new Vector3(2, 2, 2),
						Rotation = Quaternion.CreateFromYawPitchRoll(MathUtils.DEG2RAD * 45, MathUtils.DEG2RAD * 45, MathUtils.DEG2RAD * 45),
						Material = new PhongMaterial
						{
							Diffuse = new CheckerboardTexture { ColorA = new Vector3(0.9f), ColorB = new Vector3(0.1f) },
							Normal = normal,
						}
					},
					new Sphere
					{
						Position = new Vector3(3, 1, 7.5f),
						Scale = new Vector3(2, 1, 1),
						Rotation = Quaternion.CreateFromYawPitchRoll(MathUtils.DEG2RAD * 45, MathUtils.DEG2RAD * 15, MathUtils.DEG2RAD * 30),
						Radius = 5,
						Material = new ReflectiveMaterial { Roughness = new SolidColorTexture { Color = new Vector3(0.2f) } }
					},
					new Sphere
					{
						Position = new Vector3(-3, 10, 0),
						Radius = 5,
						Material = new PhongMaterial
						{
							Diffuse = new CheckerboardTexture { ColorA = new Vector3(0.9f), ColorB = new Vector3(0.1f) },
							Normal = normal,
							Scale = new Vector2(1 / 3.0f, 1)
						}
					},
					new Sphere
					{
						Position = new Vector3(-4, 2, 0),
						Radius = 2,
						Material = new LayeredMaterial
						{
							Blend = new CheckerboardTexture(),
							A = new PhongMaterial {
								Diffuse = new CheckerboardTexture
								{
									ColorA = new Vector3(0.9f),
									ColorB = new Vector3(0.1f)
								},
								Scale = new Vector2(1 / 3.0f, 1)
							},
							B = new ReflectiveMaterial(),
							Scale = new Vector2(1 / 3.0f, 1)
						}
					},
					new Sphere
					{
						Position = new Vector3(0, 2, 0),
						Radius = 2,

						Material = new LayeredMaterial
						{
							Blend = new SolidColorTexture { Color = new Vector3(0.5f) },
							A = new PhongMaterial {
								Diffuse = new CheckerboardTexture { ColorA = new Vector3(0.9f), ColorB = new Vector3(0.1f) },
								Scale = new Vector2(1 / 3.0f, 1)
							},
							B = new ReflectiveMaterial()
						}
					},
					new Sphere
					{
						Position = new Vector3(4, 2, 0),
						Radius = 2,
						Material = new ReflectiveMaterial()
					},
					new Sphere
					{
						Position = new Vector3(8, 2, 0),
						Radius = 2,
						Material = new ReflectiveMaterial
						{
							Roughness = new CheckerboardTexture
							{
								ColorA = new Vector3(0.5f)
							},
							Scale = new Vector2(1 / 3.0f, 1)
						}
					},
					new Sphere
					{
						Position = new Vector3(12, 2, 0),
						Radius = 2,
						Material = new LambertMaterial
						{
							Diffuse = new SolidColorTexture { Color = new Vector3(0, 1, 0) }
						}
					},
					new Sphere
					{
						RayMask = eRayMask.Visible | eRayMask.CastShadows,
						Position = new Vector3(16, 2, 0),
						Radius = 2,
						Material = new EmissiveMaterial
						{
							Emission = new SolidColorTexture { Color = new Vector3(0, 0, 1000) }
						}
					},
					new Sphere
					{
						RayMask = eRayMask.Visible | eRayMask.CastShadows,
						Position = new Vector3(8, 3, -6),
						Radius = 2,
						Material = new RefractiveMaterial { Ior = 1.5f, Color = new Vector3(0.8f, 1.0f, 1.0f) }
					},
					new Sphere
					{
						RayMask = eRayMask.Visible | eRayMask.CastShadows,
						Position = new Vector3(8, 3, -6),
						Radius = -1.8f,
						Material = new RefractiveMaterial { Ior = 1.5f, Color = new Vector3(0.8f, 1.0f, 1.0f)  }
					},
					new SceneObjects.Geometry.Plane
					{
						Material = new PhongMaterial
						{
							Diffuse = new CheckerboardTexture { ColorA = new Vector3(0.9f, 0.9f, 0.1f), ColorB = new Vector3(0.1f) },
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
						Material = new ReflectiveMaterial
						{
							Normal = normal,
							Color = new Vector3(1.0f, 0.2f, 0)
						}
					}
				},
				Layers = new List<ILayer>
				{
					new MaterialsLayer()
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
					layer.Progress >= layer.RenderSize
						? TimeSpan.Zero
						: layer.Progress == 0
							? TimeSpan.MaxValue
							: TimeSpan.FromMilliseconds(MathUtils.Clamp((float)elapsed.TotalMilliseconds / percent, 0, (float)TimeSpan.MaxValue.TotalMilliseconds)) * (1 - percent);

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

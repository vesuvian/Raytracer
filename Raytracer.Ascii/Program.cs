using System;
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;
using Raytracer.Layers;
using Raytracer.Materials;
using Raytracer.Materials.Textures;
using Raytracer.Parsers;
using Raytracer.SceneObjects;
using Raytracer.SceneObjects.Cameras;
using Raytracer.SceneObjects.Geometry;
using Raytracer.SceneObjects.Lights;
using Raytracer.Utils;

namespace Raytracer.Ascii
{
	public static class Program
	{
		private static int Width { get { return Console.WindowWidth; } }
		private static int Height { get { return Console.WindowHeight; } }
		private static float Aspect { get { return (float)Width / Height; } }
		private static float PixelAspect { get { return 8.0f / 16.0f; } }
		
		public static void Main()
		{
			Console.CursorVisible = false;

			BitmapTexture normal = BitmapTexture.FromPath("Resources\\TexturesCom_Wall_Stone2_3x3_1K_normal.tif");

			Scene scene = new Scene
			{
				GlobalIlluminationSamples = 4,
				Camera = new PerspectiveCamera
				{
					Position = new Vector3(5, 2, -20),
					NearPlane = 0.01f,
					FarPlane = 1000.0f,
					Fov = 40,
					Samples = int.MaxValue,
					FocalLength = 18,
					ApertureSize = 0.2f,
					Aspect = Aspect * PixelAspect
				},
				Lights = new List<ILight>
				{
					new PointLight
					{
						Position = new Vector3(10, 100, -5),
						Color = new Vector4(5, 0, 0, 1),
						Range = 200,
						SoftShadowRadius = 2,
						Falloff = eFalloff.Linear
					},
					new PointLight
					{
						Position = new Vector3(0, 10, 10),
						Color = new Vector4(0, 5, 0, 1),
						Range = 40,
						SoftShadowRadius = 2,
						Falloff = eFalloff.Linear
					},
					new PointLight
					{
						Position = new Vector3(0, 1, -20),
						Color = new Vector4(5, 5, 5, 1),
						Range = 80,
						SoftShadowRadius = 2,
						Falloff = eFalloff.Linear
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
					new Cube
					{
						Position = new Vector3(13f, 5, 0),
						Scale = new Vector3(2, 2, 2),
						Rotation = Quaternion.CreateFromYawPitchRoll(MathUtils.DEG2RAD * 45, MathUtils.DEG2RAD * 45, MathUtils.DEG2RAD * 45),
						Material = new PhongMaterial
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
						Material = new ReflectiveMaterial { Roughness = new SolidColorTexture { Color = new Vector4(0.2f) } }
					},
					new Sphere
					{
						Position = new Vector3(-3, 10, 0),
						Radius = 5,
						Material = new PhongMaterial
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
						Material = new LayeredMaterial
						{
							Blend = new CheckerboardTexture(),
							A = new PhongMaterial {
								Diffuse = new CheckerboardTexture
								{
									ColorA = new Vector4(0.9f),
									ColorB = new Vector4(0.1f)
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
							Blend = new SolidColorTexture { Color = new Vector4(0.5f) },
							A = new PhongMaterial {
								Diffuse = new CheckerboardTexture { ColorA = new Vector4(0.9f), ColorB = new Vector4(0.1f) },
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
								ColorA = new Vector4(0.5f)
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
							Diffuse = new SolidColorTexture { Color = new Vector4(0, 1, 0, 1) }
						}
					},
					new Sphere
					{
						Position = new Vector3(16, 2, 0),
						Radius = 2,
						Material = new EmissiveMaterial
						{
							Emission = new SolidColorTexture { Color = new Vector4(0, 0, 1000, 1) }
						}
					},
					new Sphere
					{
						RayMask = eRayMask.Visible,
						Position = new Vector3(8, 2, -6),
						Radius = 2,
						Material = new RefractiveMaterial { Ior = 1.5f }
					},
					new Sphere
					{
						RayMask = eRayMask.Visible,
						Position = new Vector3(8, 2, -6),
						Radius = -1.8f,
						Material = new RefractiveMaterial { Ior = 1.5f }
					},
					new SceneObjects.Geometry.Plane
					{
						Material = new PhongMaterial
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
						Material = new ReflectiveMaterial
						{
							Normal = normal
						}
					}
				},
				Layers = new List<ILayer>
				{
					new MaterialsLayer()
				}
			};

			Parallel.ForEach(scene.Layers,
			                 layer =>
			                 {
				                 Render(scene, layer);
			                 });
		}

		private static void Render(Scene scene, ILayer layer)
		{
			using (AsciiBuffer buffer = new AsciiBuffer(Width, Height))
			{
				layer.Render(scene, buffer);
			}
		}
	}
}

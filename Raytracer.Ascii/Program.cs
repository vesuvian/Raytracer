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
using Raytracer.SceneObjects.Geometry.Models;
using Raytracer.SceneObjects.Geometry.Primitives;
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
				AmbientOcclusionSamples = 16,
				AmbientOcclusionScale = 0.5f,
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
					new SphereSceneGeometry
					{
						Radius = 100000,
						RayMask = eRayMask.Visible,
						Material = new EmissiveMaterial
						{
							Emission = BitmapTexture.FromPath("Resources\\skysphere.jpg")
						}
					},
					new CylinderSceneGeometry
					{
						Position = new Vector3(5, 2, -10),
						Height = 4,
						RayMask = eRayMask.Visible | eRayMask.CastShadows,
						Material = new RefractiveMaterial
						{
							Color = new Vector3(1, 0, 0)
						}
					},
					new CubeSceneGeometry
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
					new SphereSceneGeometry
					{
						Position = new Vector3(3, 1, 7.5f),
						Scale = new Vector3(2, 1, 1),
						Rotation = Quaternion.CreateFromYawPitchRoll(MathUtils.DEG2RAD * 45, MathUtils.DEG2RAD * 15, MathUtils.DEG2RAD * 30),
						Radius = 5,
						Material = new ReflectiveMaterial { Roughness = new SolidColorTexture { Color = new Vector3(0.2f) } }
					},
					new SphereSceneGeometry
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
					new SphereSceneGeometry
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
					new SphereSceneGeometry
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
					new SphereSceneGeometry
					{
						Position = new Vector3(4, 2, 0),
						Radius = 2,
						Material = new ReflectiveMaterial()
					},
					new SphereSceneGeometry
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
					new SphereSceneGeometry
					{
						Position = new Vector3(12, 2, 0),
						Radius = 2,
						Material = new LambertMaterial
						{
							Diffuse = new SolidColorTexture { Color = new Vector3(0, 1, 0) }
						}
					},
					new SphereSceneGeometry
					{
						RayMask = eRayMask.Visible | eRayMask.CastShadows,
						Position = new Vector3(16, 2, 0),
						Radius = 2,
						Material = new EmissiveMaterial
						{
							Emission = new SolidColorTexture { Color = new Vector3(0, 0, 1000) }
						}
					},
					new SphereSceneGeometry
					{
						RayMask = eRayMask.Visible | eRayMask.CastShadows,
						Position = new Vector3(8, 3, -6),
						Radius = 2,
						Material = new RefractiveMaterial { Ior = 1.5f, Color = new Vector3(0.8f, 1.0f, 1.0f) }
					},
					new SphereSceneGeometry
					{
						RayMask = eRayMask.Visible | eRayMask.CastShadows,
						Position = new Vector3(8, 3, -6),
						Radius = -1.8f,
						Material = new RefractiveMaterial { Ior = 1.5f, Color = new Vector3(0.8f, 1.0f, 1.0f)  }
					},
					new PlaneSceneGeometry
					{
						Material = new PhongMaterial
						{
							Diffuse = new CheckerboardTexture { ColorA = new Vector3(0.9f, 0.9f, 0.1f), ColorB = new Vector3(0.1f) },
							Normal = normal,
							Scale = new Vector2(5, 5)
						}
					},
					new ModelSceneGeometry
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
            scene.Initialize();

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

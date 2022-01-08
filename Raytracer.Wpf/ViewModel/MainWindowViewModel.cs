using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Raytracer.Buffers;
using Raytracer.Extensions;
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
using Raytracer.Wpf.Utils;

namespace Raytracer.Wpf.ViewModel
{
	public sealed class MainWindowViewModel : AbstractViewModel
	{
		private const int WIDTH = 1920;
		private const int HEIGHT = 1080;

		private readonly Thread m_Worker;
		private readonly CancellationTokenSource m_CancellationTokenSource;

		private string m_Title;

		public WriteableBitmap Bitmap { get; }

		public IBuffer Buffer { get; }

		public string Title
		{
			get { return m_Title ?? string.Empty; }
			private set
			{
				if (value == m_Title)
					return;

				m_Title = value;

				RaisePropertyChanged();
			}
		}

		public ICommand CopyImageCommand { get => new RelayCommand(CopyImage); }

		/// <summary>
		/// Constructor.
		/// </summary>
		public MainWindowViewModel()
		{
			m_CancellationTokenSource = new CancellationTokenSource();

			Title = "Raytracer";
			Bitmap = new WriteableBitmap(WIDTH, HEIGHT, 96, 96, PixelFormats.Bgr32, null);

			BitmapTexture normal = BitmapTexture.FromPath("Resources\\TexturesCom_Wall_Stone2_3x3_1K_normal.tif");

			Scene scene = new Scene
			{
				GlobalIlluminationSamples = 4,
				AmbientOcclusionSamples = 4,
				AmbientOcclusionScale = 2f,
				Camera = new PerspectiveCamera
				{
					Position = new Vector3(5, 2, -20),
					NearPlane = 0.01f,
					FarPlane = 1000.0f,
					Fov = 40,
					Samples = int.MaxValue,
					FocalLength = 10,
					ApertureSize = 0.02f,
					Aspect = WIDTH / (float)HEIGHT
				},
				Lights = new List<ILight>
				{
					//new PointLight
					//{
					//	Position = new Vector3(10, 100, -5),
					//	Color = new Vector4(20000, 10000, 10000, 1) * 0.1f,
					//	SoftShadowRadius = 2,
					//	Falloff = eFalloff.Quadratic
					//},
					//new PointLight
					//{
					//	Position = new Vector3(0, 10, 10),
					//	Color = new Vector4(0, 500, 0, 1),
					//	SoftShadowRadius = 2,
					//	Falloff = eFalloff.Quadratic
					//},
					new DirectionalLight
                    {
                        Rotation = Quaternion.CreateFromYawPitchRoll(0, 135 * MathUtils.DEG2RAD, 0),
                        Color = new Vector3(0.9f),
						//SoftShadowRadius = 2
					},

                    new PointLight
                    {
                        Position = new Vector3(5, 20, 20),
                        Color = new Vector3(3000, 3000, 3000) * 2,
                        SoftShadowRadius = 2,
                        Falloff = eFalloff.Quadratic
                    }
                },
				Geometry = new List<ISceneGeometry>
				{
					//new CubeSceneGeometry
					//{
					//	Position = new Vector3(13f, 5, 0),
					//	Scale = new Vector3(2, 2, 2),
					//	Rotation = Quaternion.CreateFromYawPitchRoll(MathUtils.DEG2RAD * 45, MathUtils.DEG2RAD * 45, MathUtils.DEG2RAD * 45),
					//	Material = new PhongMaterial
					//	{
					//		Diffuse = new CheckerboardTexture { ColorA = new Vector4(0.9f), ColorB = new Vector4(0.1f) },
					//		Normal = normal,
					//	}
					//},
					//new SphereSceneGeometry
					//{
					//	Position = new Vector3(3, 1, 7.5f),
					//	Scale = new Vector3(2, 1, 1),
					//	Rotation = Quaternion.CreateFromYawPitchRoll(MathUtils.DEG2RAD * 45, MathUtils.DEG2RAD * 15, MathUtils.DEG2RAD * 30),
					//	Radius = 5,
					//	Material = new ReflectiveMaterial { Roughness = new SolidColorTexture { Color = new Vector4(0.2f) } }
					//},
					//new SphereSceneGeometry
					//{
					//	Position = new Vector3(-3, 10, 0),
					//	Radius = 5,
					//	Material = new PhongMaterial
					//	{
					//		Diffuse = new CheckerboardTexture { ColorA = new Vector4(0.9f), ColorB = new Vector4(0.1f) },
					//		Normal = normal,
					//		Scale = new Vector2(1 / 3.0f, 1)
					//	}
					//},
					//new SphereSceneGeometry
					//{
					//	Position = new Vector3(-4, 2, 0),
					//	Radius = 2,
					//	Material = new LayeredMaterial
					//	{
					//		Blend = new CheckerboardTexture(),
					//		A = new PhongMaterial {
					//			Diffuse = new CheckerboardTexture
					//			{
					//				ColorA = new Vector4(0.9f),
					//				ColorB = new Vector4(0.1f)
					//			},
					//			Scale = new Vector2(1 / 3.0f, 1)
					//		},
					//		B = new ReflectiveMaterial(),
					//		Scale = new Vector2(1 / 3.0f, 1)
					//	}
					//},
					//new SphereSceneGeometry
					//{
					//	Position = new Vector3(0, 2, 0),
					//	Radius = 2,

					//	Material = new LayeredMaterial
					//	{
					//		Blend = new SolidColorTexture { Color = new Vector4(0.5f) },
					//		A = new PhongMaterial {
					//			Diffuse = new CheckerboardTexture { ColorA = new Vector4(0.9f), ColorB = new Vector4(0.1f) },
					//			Scale = new Vector2(1 / 3.0f, 1)
					//		},
					//		B = new ReflectiveMaterial()
					//	}
					//},
					//new SphereSceneGeometry
					//{
					//	Position = new Vector3(4, 2, 0),
					//	Radius = 2,
					//	Material = new ReflectiveMaterial()
					//},
					//new SphereSceneGeometry
					//{
					//	Position = new Vector3(8, 2, 0),
					//	Radius = 2,
					//	Material = new ReflectiveMaterial
					//	{
					//		Roughness = new CheckerboardTexture
					//		{
					//			ColorA = new Vector4(0.5f)
					//		},
					//		Scale = new Vector2(1 / 3.0f, 1)
					//	}
					//},
					//new SphereSceneGeometry
					//{
					//	Position = new Vector3(12, 2, 0),
					//	Radius = 2,
					//	Material = new LambertMaterial
					//	{
					//		Diffuse = new SolidColorTexture { Color = new Vector4(0, 1, 0, 1) }
					//	}
					//},
					//new SphereSceneGeometry
					//{
					//	RayMask = eRayMask.Visible | eRayMask.CastShadows,
					//	Position = new Vector3(16, 2, 0),
					//	Radius = 2,
					//	Material = new EmissiveMaterial
					//	{
					//		Emission = new SolidColorTexture { Color = new Vector4(0, 0, 1000, 1) }
					//	}
					//},

					//new SphereSceneGeometry
					//{
					//	RayMask = eRayMask.Visible | eRayMask.CastShadows,
					//	Position = new Vector3(8, 3, -6),
					//	Radius = -1.8f,
					//	Material = new RefractiveMaterial { Ior = 1.5f, Color = new Vector4(0.8f, 1.0f, 1.0f, 1.0f)  }
					//},




					new SphereSceneGeometry
					{
						Radius = 100000,
						RayMask = eRayMask.Visible,
						Material = new EmissiveMaterial
						{
							Emission = BitmapTexture.FromPath("Resources\\skysphere.jpg")
						}
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
						Position = new Vector3(1.5f, 3, -8),
						Rotation = Quaternion.CreateFromYawPitchRoll(MathUtils.DEG2RAD * -45, MathUtils.DEG2RAD * -15, MathUtils.DEG2RAD * 30),
						Mesh = new ObjMeshParser().Parse("Resources\\teapot.obj"),
						Material = new ReflectiveMaterial
						{
							Normal = normal,
							Color = new Vector3(1.0f, 0.2f, 0) * 0.9f
						}
					},
					new SphereSceneGeometry
					{
						Position = new Vector3(9, 3, -6),
						Radius = 2,
						Material = new RefractiveMaterial
						{
							Scatter = 0,
							Ior = 1.5f,
							Color = new Vector3(0.1f, 1.0f, 0.4f) * 0.1f
						}
					},
					new CylinderSceneGeometry
					{
						Position = new Vector3(5, 2.5f, -10),
						Rotation = Quaternion.CreateFromYawPitchRoll(0, -25 * MathUtils.DEG2RAD, 25 * MathUtils.DEG2RAD),
						Height = 4,
						Material = new RefractiveMaterial
						{
							Ior = 1.5f,
							Color = new Vector3(1, 0, 0)  * 0.9f
						}
					},
				},
				Layers = new List<ILayer>
				{
					new MaterialsLayer()
				}
			};

			Random random = new Random(12345);
			for (int i = 0; i < 1000; i++)
			{
				var geometry = new SphereSceneGeometry
				{
					Position = random.NextVector3(new Vector3(-20, 0, 0), new Vector3(30, 30, 50)),
					Radius = random.NextFloat(0.5f, 2.0f),
					Material = new RefractiveMaterial
					{
						Ior = random.NextFloat(1, 2),
						Scatter = random.NextFloat(0, 5),
						Color = random.NextVector3(new Vector3(0, 0.5f, 0), new Vector3(360, 1, 0.9f)).FromHslToRgb()
					}
				};

				scene.Geometry.Add(geometry);
			}

			scene.Layers.First().OnProgressChanged += UpdateTitle;

			Buffer = new WriteableBitmapBuffer(Bitmap);

			m_Worker = new Thread(() =>
			{
				scene.Initialize(Buffer);
				scene.Layers.First().Render(scene, Buffer, m_CancellationTokenSource.Token);
			});
			m_Worker.Priority = ThreadPriority.Lowest;
			m_Worker.Start();
		}

		private void UpdateTitle(object sender, EventArgs eventArgs)
		{
			ILayer layer = (ILayer)sender;

			lock (m_Worker)
			{
				TimeSpan elapsed =
					layer.Progress >= layer.RenderSize
						? layer.End - layer.Start
						: DateTime.UtcNow - layer.Start;

				float percent = layer.RenderSize == 0 ? 0 : (layer.Progress / (float)layer.RenderSize);

				TimeSpan remaining =
					layer.Progress >= layer.RenderSize
						? TimeSpan.Zero
						: layer.Progress == 0
							? TimeSpan.MaxValue
							: TimeSpan.FromMilliseconds(MathUtils.Clamp((float)elapsed.TotalMilliseconds / percent, 0, (float)TimeSpan.MaxValue.TotalMilliseconds)) * (1 - percent);

				Title = $"Raytracer - {percent:P} ({elapsed} elapsed, {remaining} remaining)";
			}
		}

		public void Closing()
		{
			m_CancellationTokenSource.Cancel();
		}

		private void CopyImage()
		{
			Clipboard.SetImage(Bitmap);
		}
	}
}

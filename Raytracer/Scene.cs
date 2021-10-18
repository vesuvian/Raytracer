using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using Raytracer.Buffers;
using Raytracer.Geometry;
using Raytracer.Layers;
using Raytracer.Math;
using Raytracer.SceneObjects;
using Raytracer.SceneObjects.Cameras;
using Raytracer.SceneObjects.Geometry;
using Raytracer.SceneObjects.Geometry.Models;
using Raytracer.SceneObjects.Lights;
using Raytracer.Utils;

namespace Raytracer
{
	public sealed class Scene
	{
		public ICamera Camera { get; set; }
		public List<ILight> Lights { get; set; }
		public List<ISceneGeometry> Geometry { get; set; }
		public List<ILayer> Layers { get; set; }

		public int MaxReflectionRays { get; set; } = 10;
		public int GlobalIlluminationSamples { get; set; } = 4;
		public int AmbientOcclusionSamples { get; set; } = 4;
		public float AmbientOcclusionScale { get; set; } = 1;

		public BoundingVolumeHierarchy Bvh { get; private set; }

		public void Initialize(IBuffer buffer = null)
		{
			DrawGeometry(buffer);

			Bvh = new BoundingVolumeHierarchy(Geometry, maxDepth: 5, nodeAdded: s => DrawBvh(s, buffer));

			Trace.WriteLine($"{Bvh.GetNodesRecursive().Count()} BVH Nodes");
		}

		public IEnumerable<Intersection> GetIntersections(Ray ray, eRayMask mask = eRayMask.All,
		                                                  float minDelta = float.NegativeInfinity,
		                                                  float maxDelta = float.PositiveInfinity, bool ordered = true)
		{
			IEnumerable<Intersection> intersections = Bvh.GetIntersections(ray, mask, minDelta, maxDelta);

			if (ordered)
				intersections = intersections.OrderBy(i => i.RayDelta);

			return intersections;
		}

		private void DrawGeometry(IBuffer buffer)
		{
			foreach (ModelSceneGeometry model in Geometry.OfType<ModelSceneGeometry>())
			{
				foreach (Triangle triangle in model.Mesh.Triangles.Select(t => t.Multiply(model.LocalToWorld)))
				{
					DrawingUtils.DrawLine(Camera, triangle.A.Position, triangle.B.Position, buffer, Color.Green);
					DrawingUtils.DrawLine(Camera, triangle.B.Position, triangle.C.Position, buffer, Color.Green);
					DrawingUtils.DrawLine(Camera, triangle.C.Position, triangle.A.Position, buffer, Color.Green);
				}
			}
		}

		private void DrawBvh(ISceneGeometry geometry, IBuffer buffer)
		{
			var color = geometry is BoundingVolumeHierarchy ? Color.Blue : Color.Red;
			DrawingUtils.DrawAabb(Camera, geometry.Aabb, buffer, color);
		}
	}
}

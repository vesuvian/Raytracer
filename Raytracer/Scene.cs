using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading;
using Raytracer.Buffers;
using Raytracer.Extensions;
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
        public int Samples { get; set; } = 32;
        public int GlobalIlluminationSamples { get; set; } = 4;
		public int AmbientOcclusionSamples { get; set; } = 4;
		public float AmbientOcclusionScale { get; set; } = 1;

		public BoundingVolumeHierarchy Bvh { get; private set; }

		public void Initialize(IBuffer buffer = null)
		{
			if (buffer != null)
				DrawGeometry(buffer, Color.Green);

			Bvh = new BoundingVolumeHierarchy(Geometry, 10, (l, r) =>
			{
				if (buffer != null)
					DrawBvh(l, r, buffer);
			});

			Trace.WriteLine($"{Bvh.GetNodesRecursive().Count()} BVH Nodes");
		}

		public bool GetIntersection(Ray ray, out Intersection intersection, eRayMask mask = eRayMask.All,
		                            float minDelta = float.NegativeInfinity, float maxDelta = float.PositiveInfinity)
		{
			return Bvh.GetIntersection(ray, mask, out intersection, minDelta, maxDelta);
		}

		private void DrawGeometry(IBuffer buffer, Color color)
		{
			foreach (ISceneGeometry model in Geometry)
				DrawGeometry(buffer, model, color);
		}

		private void DrawGeometry(IBuffer buffer, ISceneGeometry geometry, Color color)
		{
			Mesh mesh = null;

			if (geometry is ModelSceneGeometry m)
				mesh = m.Mesh;
			else if (geometry is ModelSliceSceneGeometry s)
				mesh = s.Mesh;
			else
				return;

			foreach (Triangle triangle in mesh.Triangles.Select(t => t.Multiply(geometry.LocalToWorld)))
			{
				DrawingUtils.DrawLine(Camera, triangle.A.Position, triangle.B.Position, buffer, color);
				DrawingUtils.DrawLine(Camera, triangle.B.Position, triangle.C.Position, buffer, color);
				DrawingUtils.DrawLine(Camera, triangle.C.Position, triangle.A.Position, buffer, color);
			}
		}

		private void DrawBvh(IEnumerable<ISceneGeometry> left, IEnumerable<ISceneGeometry> right, IBuffer buffer)
		{
			HashSet<ISceneGeometry> leftSet = left.ToHashSet();
			HashSet<ISceneGeometry> rightSet = right.ToHashSet();

			foreach (ISceneGeometry item in leftSet)
				DrawGeometry(buffer, item, Color.Blue);

			foreach (ISceneGeometry item in rightSet)
				DrawGeometry(buffer, item, Color.Red);

			if (leftSet.Count > 0)
			{
				Aabb leftAabb = leftSet.Select(g => g.Aabb).Sum();
				DrawingUtils.DrawAabb(Camera, leftAabb, buffer, Color.Yellow);
			}

			if (rightSet.Count > 0)
			{
				Aabb rightAabb = rightSet.Select(g => g.Aabb).Sum();
				DrawingUtils.DrawAabb(Camera, rightAabb, buffer, Color.Yellow);
			}
		}

		private void DrawBvh(ISceneGeometry geometry, IBuffer buffer)
		{
			lock (buffer)
			{
				var color = geometry is BoundingVolumeHierarchy ? Color.Blue : Color.Red;
				DrawingUtils.DrawAabb(Camera, geometry.Aabb, buffer, color);

				Thread.Sleep(2000);
			}
		}
	}
}

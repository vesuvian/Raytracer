using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Raytracer.Extensions;
using Raytracer.Math;
using Raytracer.Parsers;

namespace Raytracer.SceneObjects.Geometry
{
	public sealed class Model : AbstractSceneGeometry
	{
		/// <summary>
		/// Sorted list of cumulative surface area to triangle index.
		/// </summary>
		private readonly List<KeyValuePair<float, int>> m_SurfaceAreaCache = new List<KeyValuePair<float, int>>();

		private Mesh m_Mesh = new Mesh();

		public Mesh Mesh
		{
			get { return m_Mesh; }
			set
			{
				m_Mesh = value;

				RebuildSurfaceAreaCache();

				// Force a rebuild of the AABB
				HandleTransformChange();
			}
		}

		public override Vector3 GetRandomPointOnSurface(Random random = null)
		{
			random ??= new Random();

			float cumulativeSurfaceArea = random.NextFloat(0, m_SurfaceAreaCache[^1].Key);
			int triangleIndex = GetNextClosestSurfaceAreaTriangle(cumulativeSurfaceArea);

			// Positions
			int vertexIndex0 = m_Mesh.Triangles[triangleIndex];
			int vertexIndex1 = m_Mesh.Triangles[triangleIndex + 1];
			int vertexIndex2 = m_Mesh.Triangles[triangleIndex + 2];

			Vector3 a = m_Mesh.Vertices[vertexIndex0];
			Vector3 b = m_Mesh.Vertices[vertexIndex1];
			Vector3 c = m_Mesh.Vertices[vertexIndex2];

			float r1 = random.NextFloat();
			float r2 = random.NextFloat();

			Vector3 output =
				new Vector3((1 - MathF.Sqrt(r1)) * a.X + (MathF.Sqrt(r1) * (1 - r2)) * b.X + (MathF.Sqrt(r1) * r2) * c.X,
				            (1 - MathF.Sqrt(r1)) * a.Y + (MathF.Sqrt(r1) * (1 - r2)) * b.Y + (MathF.Sqrt(r1) * r2) * c.Y,
				            (1 - MathF.Sqrt(r1)) * a.Z + (MathF.Sqrt(r1) * (1 - r2)) * b.Z + (MathF.Sqrt(r1) * r2) * c.Z);

			return LocalToWorld.MultiplyPoint(output);
		}

		private int GetNextClosestSurfaceAreaTriangle(float cumulativeSurfaceArea)
		{
			// Corner cases
			if (cumulativeSurfaceArea <= m_SurfaceAreaCache[0].Key)
				return m_SurfaceAreaCache[0].Value;
			if (cumulativeSurfaceArea >= m_SurfaceAreaCache[^1].Key)
				return m_SurfaceAreaCache[^1].Value;

			// Doing binary search
			int left = 0;
			int right = m_SurfaceAreaCache.Count;
			int mid = 0;

			while (left < right)
			{
				mid = (left + right) / 2;

				// If the item is less than the search amount
				if (m_SurfaceAreaCache[mid].Key < cumulativeSurfaceArea)
				{
					// If the item to the right is greater than the search amount we can return it
					if (mid < m_SurfaceAreaCache.Count - 1 && m_SurfaceAreaCache[mid + 1].Key >= cumulativeSurfaceArea)
						return m_SurfaceAreaCache[mid + 1].Value;
					// Otherwise continue searching
					left = mid + 1;
				}
				// If the item is greater than or equal to the search amount
				else
				{
					// If the item to the left is less than the search amount we can return the item
					if (mid > 0 && m_SurfaceAreaCache[mid - 1].Key < cumulativeSurfaceArea)
						return m_SurfaceAreaCache[mid].Value;
					// Otherwise continue searching
					right = mid;
				}
			}

			// Only single element left after search
			return m_SurfaceAreaCache[mid].Value;
		}

		protected override IEnumerable<Intersection> GetIntersectionsFinal(Ray ray)
		{
			// First transform the ray into local space
			ray = ray.Multiply(WorldToLocal);

			for (int triangleIndex = 0; triangleIndex < m_Mesh?.Triangles?.Count; triangleIndex += 3)
			{
				// Positions
				int vertexIndex0 = m_Mesh.Triangles[triangleIndex];
				int vertexIndex1 = m_Mesh.Triangles[triangleIndex + 1];
				int vertexIndex2 = m_Mesh.Triangles[triangleIndex + 2];

				Vector3 vertex0 = m_Mesh.Vertices[vertexIndex0];
				Vector3 vertex1 = m_Mesh.Vertices[vertexIndex1];
				Vector3 vertex2 = m_Mesh.Vertices[vertexIndex2];

				float t, u, v;
				if (!Triangle.HitTriangle(vertex0, vertex1, vertex2, ray, out t, out u, out v))
					continue;

				// Normals
				int vertexNormalIndex0 = m_Mesh.TriangleNormals[triangleIndex];
				int vertexNormalIndex1 = m_Mesh.TriangleNormals[triangleIndex + 1];
				int vertexNormalIndex2 = m_Mesh.TriangleNormals[triangleIndex + 2];

				Vector3 vertexNormal0 = m_Mesh.VertexNormals[vertexNormalIndex0];
				Vector3 vertexNormal1 = m_Mesh.VertexNormals[vertexNormalIndex1];
				Vector3 vertexNormal2 = m_Mesh.VertexNormals[vertexNormalIndex2];

				// Tangents
				int vertexTangentIndex0 = m_Mesh.TriangleTangents[triangleIndex];
				int vertexTangentIndex1 = m_Mesh.TriangleTangents[triangleIndex + 1];
				int vertexTangentIndex2 = m_Mesh.TriangleTangents[triangleIndex + 2];

				Vector3 vertexTangent0 = m_Mesh.VertexTangents[vertexTangentIndex0];
				Vector3 vertexTangent1 = m_Mesh.VertexTangents[vertexTangentIndex1];
				Vector3 vertexTangent2 = m_Mesh.VertexTangents[vertexTangentIndex2];

				Vector3 vertexBitangent0 = Vector3.Cross(vertexTangent0, vertexNormal0);
				Vector3 vertexBitangent1 = Vector3.Cross(vertexTangent1, vertexNormal1);
				Vector3 vertexBitangent2 = Vector3.Cross(vertexTangent2, vertexNormal2);

				// Uvs
				int vertexUvIndex0 = m_Mesh.TriangleUvs[triangleIndex];
				int vertexUvIndex1 = m_Mesh.TriangleUvs[triangleIndex + 1];
				int vertexUvIndex2 = m_Mesh.TriangleUvs[triangleIndex + 2];

				Vector2 vertexUv0 = m_Mesh.VertexUvs[vertexUvIndex0];
				Vector2 vertexUv1 = m_Mesh.VertexUvs[vertexUvIndex1];
				Vector2 vertexUv2 = m_Mesh.VertexUvs[vertexUvIndex2];

				Vector3 position = ray.PositionAtDelta(t);
				Vector3 normal = Triangle.GetInterpolatedVertexNormal(vertexNormal0, vertexNormal1, vertexNormal2, u, v);
				Vector3 tangent = Triangle.GetInterpolatedVertexNormal(vertexTangent0, vertexTangent1, vertexTangent2, u, v);
				Vector3 bitangent = Triangle.GetInterpolatedVertexNormal(vertexBitangent0, vertexBitangent1, vertexBitangent2, u, v);
				Vector2 uv = Triangle.GetInterpolatedVertexUv(vertexUv0, vertexUv1, vertexUv2, u, v);

				yield return new Intersection
				{
					Position = position,
					Normal = normal,
					Tangent = tangent,
					Bitangent = bitangent,
					Ray = ray,
					Uv = uv
				}.Multiply(LocalToWorld);
			}
		}

		protected override float CalculateUnscaledSurfaceArea()
		{
			float output = 0;

			for (int triangleIndex = 0; triangleIndex < m_Mesh?.Triangles?.Count; triangleIndex += 3)
			{
				// Positions
				int vertexIndex0 = m_Mesh.Triangles[triangleIndex];
				int vertexIndex1 = m_Mesh.Triangles[triangleIndex + 1];
				int vertexIndex2 = m_Mesh.Triangles[triangleIndex + 2];

				Vector3 a = m_Mesh.Vertices[vertexIndex0];
				Vector3 b = m_Mesh.Vertices[vertexIndex1];
				Vector3 c = m_Mesh.Vertices[vertexIndex2];

				output += Triangle.GetSurfaceArea(a, b, c);
			}

			return output;
		}

		protected override Aabb CalculateAabb()
		{
			return Aabb.FromPoints(LocalToWorld, m_Mesh?.Vertices ?? Enumerable.Empty<Vector3>());
		}

		private void RebuildSurfaceAreaCache()
		{
			m_SurfaceAreaCache.Clear();

			float surfaceArea = 0;

			for (int triangleIndex = 0; triangleIndex < m_Mesh?.Triangles?.Count; triangleIndex += 3)
			{
				int vertexIndex0 = m_Mesh.Triangles[triangleIndex];
				int vertexIndex1 = m_Mesh.Triangles[triangleIndex + 1];
				int vertexIndex2 = m_Mesh.Triangles[triangleIndex + 2];

				Vector3 a = m_Mesh.Vertices[vertexIndex0];
				Vector3 b = m_Mesh.Vertices[vertexIndex1];
				Vector3 c = m_Mesh.Vertices[vertexIndex2];

				surfaceArea += Triangle.GetSurfaceArea(a, b, c);

				m_SurfaceAreaCache.Add(new KeyValuePair<float, int>(surfaceArea, triangleIndex));
			}
		}
	}
}

using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Raytracer.Math;
using Raytracer.Parsers;

namespace Raytracer.SceneObjects.Geometry
{
	public sealed class Model : AbstractSceneGeometry
	{
		private Mesh m_Mesh;

		public Mesh Mesh
		{
			get { return m_Mesh; }
			set
			{
				m_Mesh = value;
				// Force a rebuild of the AABB
				HandleTransformChange();
			}
		}

		protected override IEnumerable<Intersection> GetIntersectionsFinal(Ray ray)
		{
			// First transform the ray into local space
			ray = ray.Multiply(WorldToLocal);

			for (int faceIndex = 0; faceIndex < m_Mesh?.Triangles?.Count; faceIndex += 3)
			{
				// Positions
				int vertexIndex0 = m_Mesh.Triangles[faceIndex];
				int vertexIndex1 = m_Mesh.Triangles[faceIndex + 1];
				int vertexIndex2 = m_Mesh.Triangles[faceIndex + 2];

				Vector3 vertex0 = m_Mesh.Vertices[vertexIndex0];
				Vector3 vertex1 = m_Mesh.Vertices[vertexIndex1];
				Vector3 vertex2 = m_Mesh.Vertices[vertexIndex2];

				float t, u, v;
				if (!Triangle.HitTriangle(vertex0, vertex1, vertex2, ray, out t, out u, out v))
					continue;

				// Normals
				int vertexNormalIndex0 = m_Mesh.TriangleNormals[faceIndex];
				int vertexNormalIndex1 = m_Mesh.TriangleNormals[faceIndex + 1];
				int vertexNormalIndex2 = m_Mesh.TriangleNormals[faceIndex + 2];

				Vector3 vertexNormal0 = m_Mesh.VertexNormals[vertexNormalIndex0];
				Vector3 vertexNormal1 = m_Mesh.VertexNormals[vertexNormalIndex1];
				Vector3 vertexNormal2 = m_Mesh.VertexNormals[vertexNormalIndex2];

				// Tangents
				int vertexTangentIndex0 = m_Mesh.TriangleTangents[faceIndex];
				int vertexTangentIndex1 = m_Mesh.TriangleTangents[faceIndex + 1];
				int vertexTangentIndex2 = m_Mesh.TriangleTangents[faceIndex + 2];

				Vector3 vertexTangent0 = m_Mesh.VertexTangents[vertexTangentIndex0];
				Vector3 vertexTangent1 = m_Mesh.VertexTangents[vertexTangentIndex1];
				Vector3 vertexTangent2 = m_Mesh.VertexTangents[vertexTangentIndex2];

				Vector3 vertexBitangent0 = Vector3.Cross(vertexTangent0, vertexNormal0);
				Vector3 vertexBitangent1 = Vector3.Cross(vertexTangent1, vertexNormal1);
				Vector3 vertexBitangent2 = Vector3.Cross(vertexTangent2, vertexNormal2);

				// Uvs
				int vertexUvIndex0 = m_Mesh.TriangleUvs[faceIndex];
				int vertexUvIndex1 = m_Mesh.TriangleUvs[faceIndex + 1];
				int vertexUvIndex2 = m_Mesh.TriangleUvs[faceIndex + 2];

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

			for (int faceIndex = 0; faceIndex < m_Mesh?.Triangles?.Count; faceIndex += 3)
			{
				// Positions
				int vertexIndex0 = m_Mesh.Triangles[faceIndex];
				int vertexIndex1 = m_Mesh.Triangles[faceIndex + 1];
				int vertexIndex2 = m_Mesh.Triangles[faceIndex + 2];

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
	}
}

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
		private float m_SphereRadius;
		private Vector3 m_SphereCentroid;

		public Mesh Mesh
		{
			get { return m_Mesh; }
			set
			{
				if (value == m_Mesh)
					return;

				m_Mesh = value;

				m_SphereCentroid = m_Mesh == null ? Vector3.Zero : m_Mesh.Vertices.Aggregate((sum, v) => sum + v) / m_Mesh.Vertices.Count; 
				m_SphereRadius = m_Mesh == null ? 0 : m_Mesh.Vertices.Select(v => (v - m_SphereCentroid).Length()).Max();
			}
		}

		public override IEnumerable<Intersection> GetIntersections(Ray ray)
		{
			// First transform the ray into local space
			ray = ray.Multiply(WorldToLocal);

			// Does the ray hit the sphere?
			if (!Sphere.HitSphere(m_SphereCentroid, m_SphereRadius, ray).Any())
				yield break;

			for (int faceIndex = 0; faceIndex < m_Mesh.Triangles.Count; faceIndex += 3)
			{
				int vertexIndex0 = m_Mesh.Triangles[faceIndex];
				int vertexIndex1 = m_Mesh.Triangles[faceIndex + 1];
				int vertexIndex2 = m_Mesh.Triangles[faceIndex + 2];

				Vector3 vertex0 = m_Mesh.Vertices[vertexIndex0];
				Vector3 vertex1 = m_Mesh.Vertices[vertexIndex1];
				Vector3 vertex2 = m_Mesh.Vertices[vertexIndex2];

				int vertexNormalIndex0 = m_Mesh.TriangleNormals[faceIndex];
				int vertexNormalIndex1 = m_Mesh.TriangleNormals[faceIndex + 1];
				int vertexNormalIndex2 = m_Mesh.TriangleNormals[faceIndex + 2];

				Vector3 vertexNormal0 = m_Mesh.VertexNormals[vertexNormalIndex0];
				Vector3 vertexNormal1 = m_Mesh.VertexNormals[vertexNormalIndex1];
				Vector3 vertexNormal2 = m_Mesh.VertexNormals[vertexNormalIndex2];

				float t, u, v;
				if (!Triangle.HitTriangle(vertex0, vertex1, vertex2, ray, out t, out u, out v))
					continue;

				yield return new Intersection
				{
					Position = ray.PositionAtDelta(t),
					Normal = Triangle.GetInterpolatedVertexNormal(vertexNormal0, vertexNormal1, vertexNormal2, u, v),
					RayOrigin = ray.Origin
				}.Multiply(LocalToWorld);
			}
		}
	}
}

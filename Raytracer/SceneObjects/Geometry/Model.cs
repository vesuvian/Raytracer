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

		public Mesh Mesh
		{
			get { return m_Mesh; }
			set
			{
				if (value == m_Mesh)
					return;

				m_Mesh = value;

				m_SphereRadius = m_Mesh == null ? 0 : m_Mesh.Vertices.Select(v => v.Length()).Max();
			}
		}

		public override IEnumerable<Intersection> GetIntersections(Ray ray)
		{
			// First transform the ray into local space
			ray = ray.Multiply(WorldToLocal);

			// Does the ray hit the sphere?
			if (!Sphere.HitSphere(m_SphereRadius, ray).Any())
				yield break;

			for (int faceIndex = 0; faceIndex < m_Mesh.Triangles.Count; faceIndex += 3)
			{
				int faceIndex0 = m_Mesh.Triangles[faceIndex];
				int faceIndex1 = m_Mesh.Triangles[faceIndex + 1];
				int faceIndex2 = m_Mesh.Triangles[faceIndex + 2];

				Vector3 vertex0 = m_Mesh.Vertices[faceIndex0];
				Vector3 vertex1 = m_Mesh.Vertices[faceIndex1];
				Vector3 vertex2 = m_Mesh.Vertices[faceIndex2];

				float t;
				if (!Triangle.HitTriangle(vertex0, vertex1, vertex2, ray, out t))
					continue;

				yield return new Intersection
				{
					Position = ray.PositionAtDelta(t),
					Normal = Triangle.GetNormal(vertex0, vertex1, vertex2),
					RayOrigin = ray.Origin
				}.Multiply(LocalToWorld);
			}
		}
	}
}

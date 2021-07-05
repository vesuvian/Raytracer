using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;

namespace Raytracer.Parsers
{
	public sealed class ObjMeshParser : AbstractMeshParser
	{
		public override Mesh Parse(Stream stream)
		{
			List<Vector3> vertices = new List<Vector3>();
			List<Vector3> vertexNormals = new List<Vector3>();
			List<Vector2> vertexUvs = new List<Vector2>();
			List<int> triangles = new List<int>();
			List<int> triangleNormals = new List<int>();
			List<int> triangleUvs = new List<int>();

			using (StreamReader reader = new StreamReader(stream))
			{
				while (reader.Peek() >= 0)
				{
					string line = reader.ReadLine();
					if (string.IsNullOrEmpty(line))
						continue;

					string[] split = line.Split(new char[]{}, StringSplitOptions.RemoveEmptyEntries);

					switch (split[0])
					{
						case "v":
							vertices.Add(new Vector3(float.Parse(split[1]), float.Parse(split[2]), float.Parse(split[3])));
							break;

						case "vn":
							vertexNormals.Add(Vector3.Normalize(new Vector3(float.Parse(split[1]), float.Parse(split[2]), float.Parse(split[3]))));
							break;

						case "vt":
							vertexUvs.Add(new Vector2(1 - float.Parse(split[1]), float.Parse(split[2])));
							break;

						case "f":
							// Faces are stored in the following forms:
							// f v1 v2 v3 ....
							// f v1/vt1 v2/vt2 v3/vt3 ...
							// f v1/vt1/vn1 v2/vt2/vn2 v3/vt3/vn3 ...
							// f v1//vn1 v2//vn2 v3//vn3 ...
							foreach (string faceVertex in Triangulate(split.Skip(1)))
							{
								string[] faceVertexSplit = faceVertex.Split("/");

								// Vertex is always the first item
								triangles.Add(int.Parse(faceVertexSplit[0]) - 1);

								// Second item is UV, if not empty
								if (faceVertexSplit.Length > 1 && !string.IsNullOrEmpty(faceVertexSplit[1]))
									triangleUvs.Add(int.Parse(faceVertexSplit[1]) - 1);

								// Third item is normal if not empty
								if (faceVertexSplit.Length > 2)
									triangleNormals.Add(int.Parse(faceVertexSplit[2]) - 1);
							}
							break;
					}
				}
			}

			Mesh output = new Mesh
			{
				Vertices = vertices,
				VertexNormals = vertexNormals,
				VertexUvs = vertexUvs,
				Triangles = triangles,
				TriangleNormals = triangleNormals,
				TriangleUvs = triangleUvs
			};

			FillTangents(output);

			return output;
		}

		private static void FillTangents(Mesh mesh)
		{
			List<Vector3> vertexTangents = new List<Vector3>();
			List<int> triangleTangents = new List<int>();

			Vector3[] tan1 = new Vector3[mesh.Triangles.Count];
			Vector3[] tan2 = new Vector3[mesh.Triangles.Count];

			for (int faceIndex = 0; faceIndex < mesh.Triangles.Count; faceIndex += 3)
			{
				// Positions
				int vertexIndex0 = mesh.Triangles[faceIndex];
				int vertexIndex1 = mesh.Triangles[faceIndex + 1];
				int vertexIndex2 = mesh.Triangles[faceIndex + 2];

				Vector3 vertex0 = mesh.Vertices[vertexIndex0];
				Vector3 vertex1 = mesh.Vertices[vertexIndex1];
				Vector3 vertex2 = mesh.Vertices[vertexIndex2];

				// Uvs
				int vertexUvIndex0 = mesh.TriangleUvs[faceIndex];
				int vertexUvIndex1 = mesh.TriangleUvs[faceIndex + 1];
				int vertexUvIndex2 = mesh.TriangleUvs[faceIndex + 2];

				Vector2 vertexUv0 = mesh.VertexUvs[vertexUvIndex0];
				Vector2 vertexUv1 = mesh.VertexUvs[vertexUvIndex1];
				Vector2 vertexUv2 = mesh.VertexUvs[vertexUvIndex2];

				float x1 = vertex1.X - vertex0.X;
				float x2 = vertex2.X - vertex0.X;
				float y1 = vertex1.Y - vertex0.Y;
				float y2 = vertex2.Y - vertex0.Y;
				float z1 = vertex1.Z - vertex0.Z;
				float z2 = vertex2.Z - vertex0.Z;

				float s1 = vertexUv1.X - vertexUv0.X;
				float s2 = vertexUv2.X - vertexUv0.X;
				float t1 = vertexUv1.Y - vertexUv0.Y;
				float t2 = vertexUv2.Y - vertexUv0.Y;

				float r = 1.0f / (s1 * t2 - s2 * t1);
				Vector3 sdir = new Vector3((t2 * x1 - t1 * x2) * r,
				                           (t2 * y1 - t1 * y2) * r,
				                           (t2 * z1 - t1 * z2) * r);
				Vector3 tdir = new Vector3((s1 * x2 - s2 * x1) * r,
				                           (s1 * y2 - s2 * y1) * r,
				                           (s1 * z2 - s2 * z1) * r);

				tan1[faceIndex] = sdir;
				tan1[faceIndex + 1] = sdir;
				tan1[faceIndex + 2] = sdir;

				tan2[faceIndex] = tdir;
				tan2[faceIndex + 1] = tdir;
				tan2[faceIndex + 2] = tdir;
			}
    
			for (int a = 0; a < mesh.Triangles.Count; a++)
			{
				int vertexNormalIndex0 = mesh.TriangleNormals[a];
				Vector3 vertexNormal0 = mesh.VertexNormals[vertexNormalIndex0];

				Vector3 n = vertexNormal0;
				Vector3 t = tan1[a];
        
				// Gram-Schmidt orthogonalize
				Vector3 tangent = Vector3.Normalize(t - n * Vector3.Dot(n, t));

				// Calculate handedness
				tangent =
					Vector3.Dot(Vector3.Cross(n, t), tan2[a]) < 0.0f
						? tangent
						: -tangent;

				vertexTangents.Add(tangent);
				triangleTangents.Add(a);
			}

			mesh.VertexTangents = vertexTangents;
			mesh.TriangleTangents = triangleTangents;
		}

		private static IEnumerable<T> Triangulate<T>(IEnumerable<T> vertices)
		{
			using (IEnumerator<T> iterator = vertices.GetEnumerator())
			{
				iterator.MoveNext();

				T a = iterator.Current;

				while (iterator.MoveNext())
				{
					T b = iterator.Current;
					iterator.MoveNext();
					T c = iterator.Current;

					yield return a;
					yield return b;
					yield return c;
				}
			}
		}
	}
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using Raytracer.Geometry;

namespace Raytracer.Parsers
{
	public sealed class ObjMeshParser : AbstractMeshParser
	{
		public bool FlipZ { get; set; } = true;

		public override Mesh Parse(Stream stream)
		{
			List<Vector3> vertices = new List<Vector3>();
			List<Vector3> vertexNormals = new List<Vector3>();
			List<Vector2> vertexUvs = new List<Vector2>();
            List<Vector3> vertexTangents = new List<Vector3>();
			List<int> triangles = new List<int>();
			List<int> triangleNormals = new List<int>();
			List<int> triangleUvs = new List<int>();
            List<int> triangleTangents = new List<int>();

			using (StreamReader reader = new StreamReader(stream))
			{
				while (reader.Peek() >= 0)
				{
					string line = reader.ReadLine();
					if (string.IsNullOrEmpty(line))
						continue;

					string[] split = line.Split(new char[]{}, StringSplitOptions.RemoveEmptyEntries);
					float zScalar = FlipZ ? -1 : 1;

					switch (split[0])
					{
						case "v":
							vertices.Add(new Vector3(float.Parse(split[1]), float.Parse(split[2]), float.Parse(split[3]) * zScalar));
							break;

						case "vn":
							vertexNormals.Add(Vector3.Normalize(new Vector3(float.Parse(split[1]), float.Parse(split[2]), float.Parse(split[3]) * zScalar)));
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

            FillTangents(vertices,
                         vertexNormals,
                         vertexUvs,
                         vertexTangents,
                         triangles,
                         triangleNormals,
                         triangleUvs,
                         triangleTangents);

            // Convert to triangles
            List<Triangle> output = new List<Triangle>();

            for (int index = 0; index < triangles.Count; index += 3)
            {
				// Positions
				int vertexIndex0 = triangles[index];
				int vertexIndex1 = triangles[index + 1];
				int vertexIndex2 = triangles[index + 2];
				
				Vector3 vertex0 = vertices[vertexIndex0];
				Vector3 vertex1 = vertices[vertexIndex1];
				Vector3 vertex2 = vertices[vertexIndex2];
                
				// Normals
				int vertexNormalIndex0 = triangleNormals[index];
				int vertexNormalIndex1 = triangleNormals[index + 1];
				int vertexNormalIndex2 = triangleNormals[index + 2];
				
				Vector3 vertexNormal0 = vertexNormals[vertexNormalIndex0];
				Vector3 vertexNormal1 = vertexNormals[vertexNormalIndex1];
				Vector3 vertexNormal2 = vertexNormals[vertexNormalIndex2];
				
				// Tangents
				int vertexTangentIndex0 = triangleTangents[index];
				int vertexTangentIndex1 = triangleTangents[index + 1];
				int vertexTangentIndex2 = triangleTangents[index + 2];
				
				Vector3 vertexTangent0 = vertexTangents[vertexTangentIndex0];
				Vector3 vertexTangent1 = vertexTangents[vertexTangentIndex1];
				Vector3 vertexTangent2 = vertexTangents[vertexTangentIndex2];
				
				// Uvs
				int vertexUvIndex0 = triangleUvs[index];
				int vertexUvIndex1 = triangleUvs[index + 1];
				int vertexUvIndex2 = triangleUvs[index + 2];
				
				Vector2 vertexUv0 = vertexUvs[vertexUvIndex0];
				Vector2 vertexUv1 = vertexUvs[vertexUvIndex1];
				Vector2 vertexUv2 = vertexUvs[vertexUvIndex2];

                Triangle triangle =
                    new Triangle
                    {
						A = new Vertex
                        {
							Position = vertex0,
							Normal = vertexNormal0,
                            Tangent = vertexTangent0,
							Uv = vertexUv0
                        },
                        B = new Vertex
                        {
                            Position = vertex1,
                            Normal = vertexNormal1,
                            Tangent = vertexTangent1,
                            Uv = vertexUv1
                        },
                        C = new Vertex
                        {
                            Position = vertex2,
                            Normal = vertexNormal2,
                            Tangent = vertexTangent2,
                            Uv = vertexUv2
                        }
					};

				output.Add(triangle);
            }

			return new Mesh { Triangles = output };
		}

		private static void FillTangents(List<Vector3> vertices,
                                         List<Vector3> vertexNormals,
                                         List<Vector2> vertexUvs,
                                         List<Vector3> vertexTangents,
                                         List<int> triangles,
                                         List<int> triangleNormals,
                                         List<int> triangleUvs,
                                         List<int> triangleTangents)
        {
            Vector3[] tan1 = new Vector3[triangles.Count];
			Vector3[] tan2 = new Vector3[triangles.Count];

			for (int faceIndex = 0; faceIndex < triangles.Count; faceIndex += 3)
			{
				// Positions
				int vertexIndex0 = triangles[faceIndex];
				int vertexIndex1 = triangles[faceIndex + 1];
				int vertexIndex2 = triangles[faceIndex + 2];

				Vector3 vertex0 = vertices[vertexIndex0];
				Vector3 vertex1 = vertices[vertexIndex1];
				Vector3 vertex2 = vertices[vertexIndex2];

				// Uvs
				int vertexUvIndex0 = triangleUvs[faceIndex];
				int vertexUvIndex1 = triangleUvs[faceIndex + 1];
				int vertexUvIndex2 = triangleUvs[faceIndex + 2];

				Vector2 vertexUv0 = vertexUvs[vertexUvIndex0];
				Vector2 vertexUv1 = vertexUvs[vertexUvIndex1];
				Vector2 vertexUv2 = vertexUvs[vertexUvIndex2];

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
    
			for (int a = 0; a < triangles.Count; a++)
			{
				int vertexNormalIndex0 = triangleNormals[a];
				Vector3 vertexNormal0 = vertexNormals[vertexNormalIndex0];

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

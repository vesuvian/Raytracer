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
			List<Vector3> vertexUvs = new List<Vector3>();
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
							vertexUvs.Add(new Vector3(1 - float.Parse(split[1]), float.Parse(split[2]), split.Length > 3 ? float.Parse(split[3]) : 0));
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

			return new Mesh
			{
				Vertices = vertices,
				VertexNormals = vertexNormals,
				VertexUvs = vertexUvs,
				Triangles = triangles,
				TriangleNormals = triangleNormals,
				TriangleUvs = triangleUvs
			};
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

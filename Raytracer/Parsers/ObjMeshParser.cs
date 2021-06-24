using System.Collections.Generic;
using System.IO;
using System.Numerics;

namespace Raytracer.Parsers
{
	public sealed class ObjMeshParser : AbstractMeshParser
	{
		public override Mesh Parse(Stream stream)
		{
			List<Vector3> vertices = new List<Vector3>();
			List<int> triangles = new List<int>();

			using (StreamReader reader = new StreamReader(stream))
			{
				while (reader.Peek() >= 0)
				{
					string line = reader.ReadLine();
					if (string.IsNullOrEmpty(line))
						continue;

					string[] split = line.Split();

					switch (split[0])
					{
						case "v":
							vertices.Add(new Vector3(float.Parse(split[1]), float.Parse(split[2]), float.Parse(split[3])));
							break;

						case "f":
							triangles.Add(int.Parse(split[1]) - 1);
							triangles.Add(int.Parse(split[2]) - 1);
							triangles.Add(int.Parse(split[3]) - 1);
							break;
					}
				}
			}

			return new Mesh
			{
				Vertices = vertices,
				Triangles = triangles
			};
		}
	}
}

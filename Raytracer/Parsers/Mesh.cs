using System.Collections.Generic;
using System.Numerics;

namespace Raytracer.Parsers
{
	public sealed class Mesh
	{
		public List<Vector3> Vertices { get; set; } = new List<Vector3>();
		public List<int> Triangles { get; set; } = new List<int>();
	}
}

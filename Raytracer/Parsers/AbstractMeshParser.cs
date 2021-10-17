using System.IO;
using Raytracer.Geometry;

namespace Raytracer.Parsers
{
	public abstract class AbstractMeshParser : IMeshParser
	{
		public abstract Mesh Parse(Stream stream);
	}
}

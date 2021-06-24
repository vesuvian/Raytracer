using System.IO;

namespace Raytracer.Parsers
{
	public abstract class AbstractMeshParser : IMeshParser
	{
		public abstract Mesh Parse(Stream stream);
	}
}

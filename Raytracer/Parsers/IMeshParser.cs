using System.IO;
using Raytracer.Geometry;

namespace Raytracer.Parsers
{
	public interface IMeshParser
	{
		Mesh Parse(Stream stream);
	}

	public static class ModelParserExtensions
	{
		public static Mesh Parse(this IMeshParser extends, string path)
		{
			using (FileStream stream = File.OpenRead(path))
				return extends.Parse(stream);
		}
	}
}

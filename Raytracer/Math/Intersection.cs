using System.Numerics;

namespace Raytracer.Math
{
	public struct Intersection
	{
		public Vector3 Position { get; set; }
		public Vector3 Normal { get; set; }
		public float Distance { get; set; }
	}
}

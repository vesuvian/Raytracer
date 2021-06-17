using System.Numerics;

namespace Raytracer.SceneObjects
{
	public interface ISceneObject
	{
		Vector3 Position { get; set; }
		Vector3 Scale { get; set; }
		Quaternion Rotation { get; set; }
		Matrix4x4 LocalToWorld { get; }
		Matrix4x4 WorldToLocal { get; }
	}
}

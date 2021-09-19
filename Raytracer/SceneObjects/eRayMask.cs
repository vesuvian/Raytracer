using System;

namespace Raytracer.SceneObjects
{
	[Flags]
	public enum eRayMask
	{
		None = 0,
		Visible = 1,
		CastShadows = 2,
		AmbientOcclusion = 4,
		All = Visible | CastShadows | AmbientOcclusion
	}
}

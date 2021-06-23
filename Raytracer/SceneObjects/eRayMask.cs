using System;

namespace Raytracer.SceneObjects
{
	[Flags]
	public enum eRayMask
	{
		None = 0,
		Visible = 1,
		CastShadows = 2,
		All = Visible | CastShadows
	}
}

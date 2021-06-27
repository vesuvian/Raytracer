﻿using System.Drawing;
using System.Numerics;

namespace Raytracer.Materials
{
	public interface IMaterial
	{
		Color Color { get; set; }
		Texture Diffuse { get; set; }
		Vector2 Scale { get; set; }

		Color Sample(Vector2 uv);
	}
}
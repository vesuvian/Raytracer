﻿using System.Drawing;
using System.Numerics;
using Raytracer.Utils;

namespace Raytracer.Materials
{
	public sealed class Material : AbstractMaterial
	{
		public override Color Sample(Vector2 uv)
		{
			if (Diffuse == null)
				return Color;

			float x = uv.X * Scale.Y;
			float y = uv.Y * Scale.Y;

			Color diffuse = Diffuse.Sample(x, y);
			return ColorUtils.Multiply(Color, diffuse);
		}

		public override Vector3 SampleNormal(Vector2 uv)
		{
			if (Normal == null)
				return new Vector3(0, 1, 0);

			float x = uv.X * Scale.Y;
			float y = uv.Y * Scale.Y;

			Color diffuse = Diffuse.Sample(x, y);

			return Vector3.Normalize(new Vector3(diffuse.R / 255.0f - 0.5f,
			                                     diffuse.G / 255.0f - 0.5f,
			                                     diffuse.B / -255.0f - 0.5f) * 2);
		}
	}
}

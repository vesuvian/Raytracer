using System;
using System.Collections.Generic;
using System.Drawing;
using Microsoft.Win32;

namespace Raytracer.Ascii.Extensions
{
	public static class ConsoleColorExtensions
	{
		private static readonly Dictionary<ConsoleColor, Color> s_ColorCache = new Dictionary<ConsoleColor, Color>();

		public static Color ToColor(this ConsoleColor extends)
		{
			Color color;
			if (!s_ColorCache.TryGetValue(extends, out color))
			{
				int bgr = (int)Registry.GetValue("HKEY_CURRENT_USER\\Console", $"ColorTable{(int)extends:D2}", 0);
				s_ColorCache[extends] = color = Color.FromArgb(bgr & 0xff, (bgr >> 8) & 0xff, (bgr >> 16) & 0xff);
			}

			return color;
		}
	}
}

using System;
using System.Collections.Generic;
using System.Drawing;
using Raytracer.Buffers;
using Raytracer.Utils;

namespace Raytracer.Ascii
{
	public sealed class AsciiBuffer : AbstractBuffer
	{
		private static readonly Dictionary<ConsoleColor, Color> s_ConsoleColors =
			new Dictionary<ConsoleColor, Color>
			{
				{ ConsoleColor.Black, Color.Black },
				{ ConsoleColor.DarkBlue, Color.DarkBlue },
				{ ConsoleColor.DarkGreen, Color.DarkGreen },
				{ ConsoleColor.DarkCyan, Color.DarkCyan },
				{ ConsoleColor.DarkRed, Color.DarkRed },
				{ ConsoleColor.DarkMagenta, Color.DarkMagenta },
				{ ConsoleColor.DarkYellow, Color.DarkGoldenrod },
				{ ConsoleColor.Gray, Color.Gray },
				{ ConsoleColor.DarkGray, Color.DarkGray },
				{ ConsoleColor.Blue, Color.Blue },
				{ ConsoleColor.Green, Color.Green },
				{ ConsoleColor.Cyan, Color.Cyan },
				{ ConsoleColor.Red, Color.Red },
				{ ConsoleColor.Magenta, Color.Magenta },
				{ ConsoleColor.Yellow, Color.Yellow },
				{ ConsoleColor.White, Color.White }
			};

		public override int Height { get; }

		public override int Width { get; }

		public AsciiBuffer(int width, int height)
		{
			Width = width;
			Height = height;
		}

		public override void Dispose()
		{
		}

		public override void SetPixel(int x, int y, Color color)
		{
			ConsoleColor closest = ConsoleColor.Black;
			float delta = float.MaxValue;

			foreach (var kvp in s_ConsoleColors)
			{
				float thisDelta = CompareHsl(kvp.Value, color);
				if (thisDelta >= delta)
					continue;

				closest = kvp.Key;
				delta = thisDelta;
			}

			lock (this)
			{
				Console.CursorVisible = false;
				Console.SetCursorPosition(x, y);
				Console.ForegroundColor = closest;
				Console.Write("\u2588");
			}
		}

		private float CompareHsl(Color a, Color b)
		{
			var aRgb = ColorUtils.ToVectorRgba(a);
			var bRgb = ColorUtils.ToVectorRgba(b);

			var aHsl = ColorUtils.RgbToHsl(aRgb);
			var bHsl = ColorUtils.RgbToHsl(bRgb);

			return (aHsl - bHsl).Length();
		}

		public override Color GetPixel(int x, int y)
		{
			throw new NotSupportedException();
		}
	}
}

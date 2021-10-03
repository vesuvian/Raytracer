using System;
using System.Drawing;
using System.Linq;
using System.Numerics;
using Raytracer.Ascii.Extensions;
using Raytracer.Buffers;
using Raytracer.Extensions;
using Raytracer.Utils;

namespace Raytracer.Ascii
{
	public sealed class AsciiBuffer : AbstractBuffer
	{
		private static readonly char[] s_LuminanceChars =
		{
			' ', // 0%
			'.', // 5%
			':', // 10%
			'=', // 15%
			'@', // 20%
			'\u2591', // 25%
			'\u2592', // 50%
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
			Console.SetCursorPosition(0, Height);
			Console.CursorVisible = true;
			Console.ResetColor();
		}

		public override Color GetPixel(int x, int y)
		{
			throw new NotSupportedException();
		}

		public override void SetPixel(int x, int y, Color color)
		{
			if (x > Width || x < 0)
				throw new ArgumentOutOfRangeException(nameof(x));

			if (y > Height || y < 0)
				throw new ArgumentOutOfRangeException(nameof(y));

			ConsoleColor background;
			ConsoleColor foreground;
			char character;

			GetConsoleColor(color, out background, out foreground, out character);

			lock (this)
			{
				Console.CursorVisible = false;
				Console.SetCursorPosition(x, y);
				Console.BackgroundColor = background;
				Console.ForegroundColor = foreground;
				Console.Write(character);
				Console.SetWindowPosition(0, 0);
			}
		}

		private static void GetConsoleColor(Color color, out ConsoleColor background, out ConsoleColor foreground, out char character)
		{
			Vector3 whiteRgb = ConsoleColor.White.ToColor().ToRgb();
			Vector3 colorRgb = color.ToRgb();
			Vector3 colorLab = colorRgb.FromRgbToLab(whiteRgb);

			ConsoleColor consoleA = ConsoleColor.Black;
			ConsoleColor consoleB = ConsoleColor.Black;
			float blend = 0;
			float smallestDelta = float.MaxValue;
			
			foreach (var a in Enum.GetValues<ConsoleColor>())
			{
				Vector3 aRgb = a.ToColor().ToRgb();
				Vector3 aLab = aRgb.FromRgbToLab(whiteRgb);

				foreach (var b in Enum.GetValues<ConsoleColor>().Where(c => c != a))
				{
					Vector3 bRgb = b.ToColor().ToRgb();
					Vector3 bLab = bRgb.FromRgbToLab(whiteRgb);

					// Treat a-b as a line. Find the closest point from the line to the target color
					float t;
					Vector3 closest = MathUtils.ClosestPointOnLine(aLab, bLab, colorLab, out t);
					if (t < 0 || t > 1)
						continue;

					float thisDelta = (closest - colorLab).Length();
					if (thisDelta > smallestDelta)
						continue;

					// Found the closest match so far
					smallestDelta = thisDelta;
					consoleA = a;
					consoleB = b;
					blend = t;

					if (System.Math.Abs(smallestDelta) < 0.01f)
						break;
				}
			}

			// Perform the blend
			bool flip = blend >= 0.5f;

			int index =
				flip
					? (int)System.Math.Round((1 - blend) * 2 * (s_LuminanceChars.Length - 1))
					: (int)System.Math.Round(blend * 2 * (s_LuminanceChars.Length - 1));

			character = s_LuminanceChars[index];

			background = flip ? consoleB : consoleA;
			foreground = flip ? consoleA : consoleB;
		}
	}
}

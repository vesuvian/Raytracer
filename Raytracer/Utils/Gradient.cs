using System;
using System.Collections.Generic;
using System.Numerics;
using Raytracer.Extensions;

namespace Raytracer.Utils
{
    public sealed class Gradient
    {
        private readonly List<KeyValuePair<float, Vector4>> m_KeyValuePairs = new();

        /// <summary>
        /// Adds the color to the given position.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="value"></param>
        public void Add(float position, Vector4 value)
        {
            if (position < 0 || position > 1)
                throw new ArgumentOutOfRangeException(nameof(position));

            var index = FindIndex(position);
            m_KeyValuePairs.Insert(index + 1, new KeyValuePair<float, Vector4>(position, value));
        }

        /// <summary>
        /// Samples the color at the given position.
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public Vector4 Sample(float position)
        {
            if (m_KeyValuePairs.Count == 0)
                return new Vector4(0, 0, 0, 1);

            var index = FindIndex(position);

            // If there's no prior color to sample then copy the first color
            var (leftPosition, leftValue) =
                index == -1
                    ? new(0, m_KeyValuePairs[index].Value)
                    : m_KeyValuePairs[index];

            // If there's no subsequent color to sample then copy the last color
            var (rightPosition, rightValue) =
                index + 1 >= m_KeyValuePairs.Count
                    ? new(1, m_KeyValuePairs[^1].Value)
                    : m_KeyValuePairs[index + 1];

            // Remap the position between the two items
            position = MathUtils.MapRange(0, 1, leftPosition, rightPosition, position);
            position = MathUtils.Clamp(position, 0, 1);

            return LerpOklab(leftValue, rightValue, position);
        }

        /// <summary>
        /// Given a position between 0 and 1, returns the last index of the KVP prior or equal to the position.
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        private int FindIndex(float position)
        {
            return m_KeyValuePairs.FindLastIndex(kvp => kvp.Key <= position);
        }

        /// <summary>
        /// Returns the color between the two points, lerping in Oklab color space.
        /// </summary>
        /// <param name="rgbA"></param>
        /// <param name="rgbB"></param>
        /// <param name="delta"></param>
        /// <returns></returns>
        public static Vector4 LerpOklab(Vector4 rgbA, Vector4 rgbB, float delta)
        {
            var oklabA = rgbA.FromRgbaToOklaba();
            var oklabB = rgbB.FromRgbaToOklaba();
            var oklabOut = Vector4.Lerp(oklabA, oklabB, delta);
            return oklabOut.FromOklabaToRgba();
        }
    }
}

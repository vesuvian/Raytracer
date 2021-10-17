using System.Diagnostics;
using System.Numerics;
using Raytracer.Extensions;

namespace Raytracer.Geometry
{
    [DebuggerDisplay("Position = {Position}")]
    public record Vertex
    {
        public Vector3 Position { get; set; }
        public Vector3 Normal { get; set; }
        public Vector3 Tangent { get; set; }
        public Vector2 Uv { get; set; }

        public Vector3 Bitangent { get { return Vector3.Cross(Tangent, Normal); } }

        public Vertex Multiply(Matrix4x4 matrix)
        {
            return new Vertex
            {
                Position = matrix.MultiplyPoint(Position),
                Normal = matrix.MultiplyNormal(Normal),
                Tangent = Vector3.Normalize(matrix.MultiplyDirection(Tangent)),
                Uv = Uv
            };
        }
    }
}

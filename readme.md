# I Wrote a Raytracer in C#
The only dependency is System.Drawing.Common.

It is very slow!

I use Parallel.ForEach to parallelize the rays, but ultimately we are doing billions of recursive vector operations on the CPU.

# How to Run It
1. Clone the repository.
2. Open the Solution in Visual Studio 2019 or later with .Net 5 SDK installed.
3. Set Raytracer.Wpf as the startup project.
4. Run the project.

# Projects
- Raytracer - Common library containing all of the math, scene descriptions, parsers, and utilities.
- Raytracer.Ascii - Renders scenes into a command line window in "realtime", looks kinda cool.
- Raytracer.Cmd - Renders scenes to bitmap file via command line.
- Raytracer.Wpf - Renders scenes to a window in "realtime".
- Raytracer.Tests - Minimal NUnit tests for Raytracer, helps verify 2 + 2 still equals 4.

# Scene Features
- Cameras
  - Orthographic
  - Perspective
- Lights
  - Ambient
  - Directional
  - Point
- Geometry
  - Primitives
    - Capsule
    - Cube
    - Cylinder
    - Disc
    - Plane
    - Quad
    - Sphere
    - Triangle
  - Models
    - OBJ
  - CSG (Constructive Solid Geometry)
    - Difference
    - Intersection
    - Union
- Materials
  - Emissive
  - Lambert
  - Layered
  - Phong
  - Reflective
  - Refractive
- Textures
  - Bitmap
  - Checkerboard
  - Noise
  - Random
  - Solid Color

# Renderer Features
- Texture Mapping
- Normal Mapping
- Anti-aliasing
- Global Illumination
- Ambient Occlusion
- Gamma correction
- Depth of Field
- Soft shadows
- Translucent shadows
- Subsurface Scattering
- Median Filtering
- Successive Refinement
- Bilinear Texture Filtering
- Bounding Volume Hierarchy

# Examples
Translucent shadows, depth of field, subsurface scattering, normal mapping
![Translucent Example Image](https://github.com/vesuvian/Raytracer/blob/main/example_scattering.png?raw=true)

Stacked CSG Union, Intersection, Difference operations
![Translucent Example Image](https://github.com/vesuvian/Raytracer/blob/main/example_csg.png?raw=true)

ASCII buffer - Translating light values to a mix of foreground and background terminal colours
![Translucent Example Image](https://github.com/vesuvian/Raytracer/blob/main/example_ascii.png?raw=true)

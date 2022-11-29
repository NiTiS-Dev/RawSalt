global using ptr = System.UIntPtr;
global using sptr = System.IntPtr;
global using Buffer = RawSalt.Graphics.Memory.Buffer;
global using Shader = RawSalt.Graphics.Shaders.Shader;
global using Texture = RawSalt.Graphics.Textures.Texture;
global using VertexArray = RawSalt.Graphics.Memory.VertexArray;
global using Color32 = RawSalt.Structs.Color32;

//Quaternion
global using quat = System.Numerics.Quaternion;

//Matrices
global using mat4 = System.Numerics.Matrix4x4;
global using mat3 = Silk.NET.Maths.Matrix3X3<float>;

//Vectors
global using vec2 = System.Numerics.Vector2;
global using vec2i = Silk.NET.Maths.Vector2D<int>;
global using vec2u = Silk.NET.Maths.Vector2D<uint>;
global using vec3 = System.Numerics.Vector3;
global using vec3i = Silk.NET.Maths.Vector3D<int>;
global using vec3u = Silk.NET.Maths.Vector3D<uint>;
global using vec4 = System.Numerics.Vector4;
global using vec4i = Silk.NET.Maths.Vector4D<int>;
global using vec4u = Silk.NET.Maths.Vector4D<uint>;

namespace RawSalt;
using Silk.NET.OpenGL;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace RawSalt.Graphics.Shaders;

/// <summary>
/// OpenGL Shader program object
/// </summary>
public readonly struct Shader
{
	private const int UniformNameMaxSize = 16;

	/// <summary>
	/// Shader handle
	/// </summary>
	public readonly uint Handle;
	private readonly IReadOnlyDictionary<string, int> uniforms;
	/// <summary>
	/// Create Shader object
	/// </summary>
	/// <param name="handle">Shader name</param>
	/// <param name="uniforms">Uniform mapping dictionary</param>
	public Shader(uint handle, IReadOnlyDictionary<string, int> uniforms)
	{
		this.Handle = handle;
		this.uniforms = uniforms;
	}
	public Shader(Shader original)
	{
		Handle = original.Handle;
		uniforms = original.uniforms;
	}
	private static uint CreateShaderPart(GL gl, string code, ShaderType type)
	{
		uint handle = gl.CreateShader(type);

		gl.ShaderSource(handle, code);
		gl.CompileShader(handle);
		string infoLog = gl.GetShaderInfoLog(handle);
		if (!string.IsNullOrWhiteSpace(infoLog))
			throw new ShaderException(infoLog);

		return handle;
	}
	public int UniformLocation(string name)
	{
		int retusa = - 1;
		
		if (!uniforms.TryGetValue(name, out retusa))
		{
#if DEBUG
			Console.WriteLine("Uniform not founded: " + name);
#endif
		}

		
		return retusa;
		/*
		int _ = gl.GetUniformLocation(Handle, name);

		return _;
		*/
	}
	public void Use(GL gl)
	{
		gl.UseProgram(Handle);
	}
	public void UniformTex(GL gl, string name, TextureUnit texutre)
	{
		gl.Uniform1(UniformLocation(name), texutre - TextureUnit.Texture0);
	}
	public void UniformTex(GL gl, string name, int texutre)
	{
		gl.Uniform1(UniformLocation(name), texutre);
	}
	[DebuggerStepThrough]
	public void UniformInt(GL gl, string name, int i)
	{
		gl.Uniform1(UniformLocation(name), i);
	}
	[DebuggerStepThrough]
	public void UniformSingle(GL gl, string name, float i)
	{
		gl.Uniform1(UniformLocation(name), i);
	}
	[DebuggerStepThrough]
	public unsafe void Uniform4(GL gl, string name, vec4 vec)
	{
		gl.Uniform4(UniformLocation(name), 1, (float*)&vec);
	}
	[DebuggerStepThrough]
	public unsafe void Uniform4(GL gl, string name, Color32 vec)
	{
		vec4 vec2 = (vec4)vec;
		
		gl.Uniform4(UniformLocation(name), 1, (float*)&vec2);
	}
	[DebuggerStepThrough]
	public unsafe void UniformMat4(GL gl, string name, mat4 mat)
	{
		gl.UniformMatrix4(UniformLocation(name), 1, false, (float*)&mat);
	}
	[DebuggerStepThrough]
	public unsafe void UniformMat4(GL gl, string name, NiTiS.Math.Mat4<float> mat)
	{
		gl.UniformMatrix4(UniformLocation(name), 1, false, (float*)&mat);
	}
	/// <summary>
	/// Creates Shader program from vertex and fragment code
	/// </summary>
	/// <param name="gl">OpenGL</param>
	/// <param name="vertex">Vertex code</param>
	/// <param name="fragment">Fragment code</param>
	/// <returns>Created shader program</returns>
	/// <exception cref="ShaderException">Shader code is invalid</exception>
	public static unsafe Shader Create(GL gl, string vertex, string fragment)
	{
		uint shaderHandle;

		shaderHandle = gl.CreateProgram();



		uint vertHandle, fragHandle;

		vertHandle = CreateShaderPart(gl, vertex, ShaderType.VertexShader);
		fragHandle = CreateShaderPart(gl, fragment, ShaderType.FragmentShader);

		gl.AttachShader(shaderHandle, vertHandle);
		gl.AttachShader(shaderHandle, fragHandle);
		gl.LinkProgram(shaderHandle);

		gl.GetProgram(shaderHandle, ProgramPropertyARB.LinkStatus, out int status);

		if (status == 0)
			throw new ShaderException(gl.GetProgramInfoLog(shaderHandle));

		gl.DetachShader(shaderHandle, vertHandle);
		gl.DetachShader(shaderHandle, fragHandle);
		gl.DeleteShader(vertHandle);
		gl.DeleteShader(fragHandle);
		int uniformCount;

		gl.GetProgram(shaderHandle, ProgramPropertyARB.ActiveUniforms, &uniformCount);
		
		Dictionary<string, int> uniforms = new Dictionary<string, int>(uniformCount);

		Span<byte> bytes = stackalloc byte[UniformNameMaxSize];

		for (uint x=0; x<uniformCount; x++)
		{
			UniformType type;
			int size;
			uint len;

			gl.GetActiveUniform(shaderHandle, x, &len, &size, &type, bytes);
			string uName;

			uName = new(Encoding.UTF8.GetString(bytes.ToArray(), 0, (int)len));

			uniforms[uName] = (int)x;
		}


		return new(shaderHandle, uniforms);
	}
	public void Dispose(GL gl)
		=> gl.DeleteProgram(Handle);
}
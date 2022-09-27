using Silk.NET.OpenGL;
using System;

namespace RawSalt.Graphics.Shaders;

/// <summary>
/// OpenGL Shader program object
/// </summary>
public readonly struct Shader
{
	/// <summary>
	/// Shader handle
	/// </summary>
	public readonly uint Handle;
	/// <summary>
	/// Create Shader object
	/// </summary>
	/// <param name="handle">Shader name</param>
	public Shader(uint handle)
	{
		Handle = handle;
	}
	private readonly uint CreateShaderPart(GL gl, string code, ShaderType type)
	{
		uint handle = gl.CreateShader(type);

		gl.ShaderSource(handle, code);
		gl.CompileShader(handle);
		string infoLog = gl.GetShaderInfoLog(handle);
		if (!String.IsNullOrWhiteSpace(infoLog))
			throw new ShaderException(infoLog);

		return handle;
	}
	/// <summary>
	/// Creates Shader program from vertex and fragment code
	/// </summary>
	/// <param name="gl">OpenGL</param>
	/// <param name="vertex">Vertex code</param>
	/// <param name="fragment">Fragment code</param>
	/// <returns>Created shader program</returns>
	/// <exception cref="ShaderException">Shader code is invalid</exception>
	public static Shader Create(GL gl, string vertex, string fragment)
	{
		Shader shader;

		shader = new(gl.CreateProgram());

		uint vertHandle, fragHandle;

		vertHandle = shader.CreateShaderPart(gl, vertex, ShaderType.VertexShader);
		fragHandle = shader.CreateShaderPart(gl, fragment, ShaderType.FragmentShader);

		gl.AttachShader(shader.Handle, vertHandle);
		gl.AttachShader(shader.Handle, fragHandle);
		gl.LinkProgram(shader.Handle);

		gl.GetProgram(shader.Handle, ProgramPropertyARB.LinkStatus, out int status);

		if (status == 0)
			throw new ShaderException(gl.GetProgramInfoLog(shader.Handle));

		gl.DetachShader(shader.Handle, vertHandle);
		gl.DetachShader(shader.Handle, fragHandle);
		gl.DeleteShader(vertHandle);
		gl.DeleteShader(fragHandle);

		return shader;
	}
	/// <summary>
	/// Creates Shader program from vertex, geometry and fragment code
	/// </summary>
	/// <param name="gl">OpenGL</param>
	/// <param name="vertex">Vertex code</param>
	/// <param name="geometry">Geometry code</param>
	/// <param name="fragment">Fragment code</param>
	/// <returns>Created shader program</returns>
	/// <exception cref="ShaderException">Shader code is invalid</exception>
	public static Shader Create(GL gl, string vertex, string geometry, string fragment)
	{
		Shader shader;

		shader = new(gl.CreateProgram());

		uint vertHandle, geomHandle, fragHandle;

		vertHandle = shader.CreateShaderPart(gl, vertex, ShaderType.VertexShader);
		geomHandle = shader.CreateShaderPart(gl, geometry, ShaderType.GeometryShader);
		fragHandle = shader.CreateShaderPart(gl, fragment, ShaderType.FragmentShader);

		gl.AttachShader(shader.Handle, vertHandle);
		gl.AttachShader(shader.Handle, geomHandle);
		gl.AttachShader(shader.Handle, fragHandle);
		gl.LinkProgram(shader.Handle);

		gl.GetProgram(shader.Handle, ProgramPropertyARB.LinkStatus, out int status);

		if (status == 0)
			throw new ShaderException(gl.GetProgramInfoLog(shader.Handle));

		gl.DetachShader(shader.Handle, vertHandle);
		gl.DetachShader(shader.Handle, geomHandle);
		gl.DetachShader(shader.Handle, fragHandle);
		gl.DeleteShader(vertHandle);
		gl.DeleteShader(geomHandle);
		gl.DeleteShader(fragHandle);

		return shader;
	}
}
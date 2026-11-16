using System.Numerics;

namespace RawSalt.Graphics.Rendering;

/// <summary>
/// Matrices data of camera.
/// </summary>
/// <param name="View">View matrix of camera.</param>
/// <param name="Projection">Projection matrix of camera.</param>
public record struct CameraData(Matrix4x4 View, Matrix4x4 Projection);
using NiTiS.Native.Loaders;
using System;
using System.IO;
using System.Reflection;
using System.Text;

namespace NiTiS.Native;

public static unsafe class NativeAPI
{
	/// <summary>
	/// Initialize static API types
	/// </summary>
	/// <param name="tAPI">Type to initialize</param>
	/// <param name="context">[Optional] API context</param>
	/// <exception cref="APIException"></exception>
	public static void Initialize(Type tAPI, delegate* unmanaged[Stdcall]<CString, void*> context = null)
	{
		NativeAPIAttribute api = tAPI.GetCustomAttribute<NativeAPIAttribute>();

		if (api is null)
			throw new APIException($"Type {tAPI.FullName} has no one {typeof(NativeAPIAttribute).FullName} attribute");

		// Init via context
		if (api.APIType is APIType.Contextual)
		{
			if (context is null)
				throw new APIException("Contextual API has no context");

			const int BufferSize = 48;

			Span<byte> nameBuffer = stackalloc byte[BufferSize];
			fixed (byte* pNameBuffer = nameBuffer)
			{
				foreach (FieldInfo fi in tAPI.GetRuntimeFields())
				{
					Console.WriteLine(fi);
					if (fi.IsLiteral)
						continue;

					string methodName = fi.GetCustomAttribute<NativeNameAttribute>()?.EntryPoint ?? api.MethodPrefix + fi.Name;

					byte[] buffer = Encoding.UTF8.GetBytes(methodName);

					if (buffer.Length >= BufferSize)
						throw new APIException($"Method {methodName} has too long name");

					MemoryExtensions.CopyTo(buffer, nameBuffer);

					nameBuffer[buffer.Length] = 0; // aka '\0'

					nint procaddress;

					procaddress = (nint)context(pNameBuffer);

					if (procaddress is 0)
						throw new APIException($"Context has no one method called {methodName}");

					// Boxing );
					fi.SetValue(null, procaddress, BindingFlags.ExactBinding, null, null);
				}
			}
		}
		// Init via lib
		else
		{
			NativeLibraryLoader loader = NativeLibraryLoader.GetPlatformDefaultLoader();

			Type tImport = api.ContainerType ?? tAPI.GetNestedType("__import", BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic);

			LibraryHandle* handle;

			if (tImport.IsAssignableTo(typeof(INativeLibraryContainer)))
			{
				INativeLibraryContainer container = Activator.CreateInstance(tImport) as INativeLibraryContainer;

				string libFileName = container.Machine();

				if (container.SearchType is LibFileSearchPath.ByPathVariable)
				{
					libFileName = new FileInfo(libFileName).FullName;
					
					handle = loader.LoadLibrary(libFileName);
				}
				else
				{
					handle = loader.LoadLibrary(libFileName);

					if (handle is null)
					{
						libFileName = Path.Combine(loader.AlternatePath, libFileName);
						handle = loader.LoadLibrary(libFileName);
					}
				}

				foreach (FieldInfo fi in tAPI.GetRuntimeFields())
				{
					if (fi.IsLiteral)
						continue;

					nint procaddress = (nint)loader.GetMethodAddress(handle, fi.GetCustomAttribute<NativeNameAttribute>()?.EntryPoint ?? api.MethodPrefix + fi.Name);

					fi.SetValue(null, procaddress, BindingFlags.DeclaredOnly, null, null);
				}
			}
		}
	}
}
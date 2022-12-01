using NiTiS.Native.Loaders;
using System;
using System.IO;
using System.Reflection;

namespace NiTiS.Native;

public static unsafe class NativeAPI
{
	public static void Initialize(Type sType)
	{
		NativeLibraryLoader loader = NativeLibraryLoader.GetPlatformDefaultLoader();

		string methodPrefix = sType.GetCustomAttribute<NativeAPIAttribute>()?.MethodPrefix;

		Type containerType = sType.GetCustomAttribute<LibraryContainerAttribute>().Container;

		if (containerType.IsAssignableTo(typeof(INativeLibraryContainer)))
		{
			INativeLibraryContainer container = (INativeLibraryContainer)Activator.CreateInstance(containerType);

			string libName = container.Machine();

			LibraryHandle* handle = loader.LoadLibrary(libName);

			if (handle is null)
			{
				string newName = Path.Combine(loader.AlternatePath, libName);

				handle = loader.LoadLibrary(newName);

				if (handle != null)
				{
					foreach (FieldInfo fi in sType.GetRuntimeFields())
					{
						string entryPoint = fi.GetCustomAttribute<NativeNameAttribute>()?.EntryPoint ?? methodPrefix + fi.Name;

						fi.SetValue(null, (nint)loader.GetMethodAddress(handle, entryPoint));
					}

					return;
				}
			}
			
			throw new Exception("Unable to load: " + libName);
		}
		else
			throw new Exception($"API has no {nameof(INativeLibraryContainer)}");
	}
}
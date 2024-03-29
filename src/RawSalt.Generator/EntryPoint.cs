using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using DotLiquid;

while (!Directory.GetDirectories(Environment.CurrentDirectory).Any(d => Path.GetFileName(d) is ".git/" or ".git" or ".git\\"))
{
	Environment.CurrentDirectory = Directory.GetParent(Environment.CurrentDirectory)!.FullName;
}

Dictionary<string, Dictionary<string, object>> contexts = new()
{
	["src/RawSalt/Mathematics/Geometry/IVec4.cs"]	= CreateVectorContext("int", 4),
	["src/RawSalt/Mathematics/Geometry/UVec4.cs"]	= CreateVectorContext("uint", 4),
	["src/RawSalt/Mathematics/Geometry/Vec4.cs"]	= CreateVectorContext("float", 4),
	["src/RawSalt/Mathematics/Geometry/DVec4.cs"]	= CreateVectorContext("double", 4),
	["src/RawSalt/Mathematics/Geometry/BVec4.cs"]	= CreateVectorContext("bool", 4),

	["src/RawSalt/Mathematics/Geometry/IVec3.cs"]	= CreateVectorContext("int", 3),
	["src/RawSalt/Mathematics/Geometry/UVec3.cs"]	= CreateVectorContext("uint", 3),
	["src/RawSalt/Mathematics/Geometry/Vec3.cs"]	= CreateVectorContext("float", 3),
	["src/RawSalt/Mathematics/Geometry/DVec3.cs"]	= CreateVectorContext("double", 3),
	["src/RawSalt/Mathematics/Geometry/BVec3.cs"]	= CreateVectorContext("bool", 3),

	["src/RawSalt/Mathematics/Geometry/IVec2.cs"]	= CreateVectorContext("int", 2),
	["src/RawSalt/Mathematics/Geometry/UVec2.cs"]	= CreateVectorContext("uint", 2),
	["src/RawSalt/Mathematics/Geometry/Vec2.cs"]	= CreateVectorContext("float", 2),
	["src/RawSalt/Mathematics/Geometry/DVec2.cs"]	= CreateVectorContext("double", 2),
	["src/RawSalt/Mathematics/Geometry/BVec2.cs"]	= CreateVectorContext("bool", 2),

	//["src/RawSalt/Mathematics/Geometry/DMat4x4.cs"]	= CreateMatrixContext("double", 4, 4),
	//["src/RawSalt/Mathematics/Geometry/Mat4x4.cs"]	= CreateMatrixContext("float", 4, 4),
	
	//["src/RawSalt/Mathematics/Geometry/DMat3x3.cs"]	= CreateMatrixContext("double", 3, 3),
	//["src/RawSalt/Mathematics/Geometry/Mat3x3.cs"]	= CreateMatrixContext("float", 3, 3),
};

foreach (KeyValuePair<string, Dictionary<string, object>> context in contexts)
{
	File.Delete(context.Key);

	Template templ = Template.Parse(File.ReadAllText(Path.Combine(Environment.CurrentDirectory, "src/RawSalt.Generator/templates", (context.Value["GenerationFile"] as string)!)));

	File.WriteAllText(context.Key, templ.Render(Hash.FromDictionary(context.Value)));
}

static Dictionary<string, object> CreateVectorContext(string type, int size)
{
	return new Dictionary<string, object>()
	{
		["GenerationFile"] = "vector.cs.liquid",
		["Count"] = size,
		["Type"] = type,
		["typed_arguments"] = new string[] { type + " x", type + " y", type + " z", type + " w", }.Take((int)size).ToArray(),
		["this_arguments"] = new string[] { "this.x", "this.y", "this.z", "this.w", }.Take((int)size).ToArray(),
		["arguments"] = new string[] { "x", "y", "z", "w", }.Take((int)size).ToArray(),
		["all_arguments"] = new string[] { "all", "all", "all", "all", }.Take((int)size).ToArray(),
		["out_typed_arguments"] = new string[] { "out " + type + " x", "out " + type + " y", "out " + type + " z", "out " + type + " w", }.Take((int)size).ToArray(),
	};
}

static Dictionary<string, object> CreateMatrixContext(string type, int rows, int columns)
{
	var dict = new Dictionary<string, object>()
	{
		["GenerationFile"] = "matrix.cs.liquid",
		["Count"] = rows * columns,
		["elements"] = GenerateRange(rows * columns),
		["Rows"] = rows,
		["Columns"] = columns,
		["Type"] = type,
		["columns_indexes"] = GenerateRange(columns),
		["rows_indexes"] = GenerateRange(rows),
	};
	
	if (rows == columns)
	{
		dict["is_identity"] = true;
		dict["identity_elements"] = new string[] { "M11", "M22", "M33", "M44" }.Take(rows * columns / rows);
	}

	return dict;
}

static int[] GenerateRange(int count)
{
	int[] ints = new int[count];

	for (int i = 0; i < count; i++)
	{
		ints[i] = (i);
	}

	return ints;
}
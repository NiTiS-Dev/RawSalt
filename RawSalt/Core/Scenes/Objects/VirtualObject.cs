namespace RawSalt.Core.Scenes.Objects;

/// <summary>
/// Scene object base
/// </summary>
public class VirtualObject : INamedObject
{
    private Scene stage;
    public string Name { get; set; }

    public VirtualObject(Scene stage, string name)
    {
        this.stage = stage;
        Name = name;
    }

}

namespace BuilderGenerator.Generation;

public class ClassProperty
{
    public string Name { get; }
    public string TypeName { get; }

    public ClassProperty(string name, string typeName)
    {
        Name = name;
        TypeName = typeName;
    }
}

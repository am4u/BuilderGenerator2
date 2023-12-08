using System;

namespace BuilderGenerator.Generation;

public class ClassProperty : IEquatable<ClassProperty>
{
    public string Name { get; }
    public string TypeName { get; }

    public ClassProperty(string name, string typeName)
    {
        Name = name;
        TypeName = typeName;
    }

    public override int GetHashCode()
    {
        unchecked
        {
            return (Name.GetHashCode() * 397);
        }
    }

    public bool Equals(ClassProperty? other)
    {
        if (ReferenceEquals(null, other))
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return Name == other.Name;
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj))
        {
            return false;
        }

        if (ReferenceEquals(this, obj))
        {
            return true;
        }

        if (obj.GetType() != GetType())
        {
            return false;
        }

        return Equals((ClassProperty)obj);
    }
}

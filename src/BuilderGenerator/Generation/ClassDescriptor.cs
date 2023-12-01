using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace BuilderGenerator.Generation;

public class ClassDescriptor
{
    private readonly INamedTypeSymbol _namedTypeSymbol;

    public string Name { get; }

    public string FullName { get; }

    public bool IsRecord { get; }

    public ClassDescriptor(TypedConstant typedConstant)
    {
        Name = ((ISymbol)typedConstant.Value!).Name;
        FullName = typedConstant.Value!.ToString();

        _namedTypeSymbol = ((INamedTypeSymbol)typedConstant.Value);
        IsRecord = _namedTypeSymbol.IsRecord;
    }

    public List<ClassProperty> GetProperties(bool includeInternal)
    {
        return GetPropertySymbols(_namedTypeSymbol, includeInternal)
            .Select<IPropertySymbol, ClassProperty>(x => new ClassProperty(x.Name, x.Type.ToString()))
            .Distinct()
            .OrderBy(x => x.Name)
            .ToList();
    }

    private static IEnumerable<IPropertySymbol> GetPropertySymbols(INamedTypeSymbol namedTypeSymbol, bool includeInternals)
    {
        var symbols = namedTypeSymbol
            .GetMembers()
            .OfType<IPropertySymbol>()
            .Where(x => x.SetMethod is not null
                && (x.SetMethod.DeclaredAccessibility == Accessibility.Public || (includeInternals && x.SetMethod.DeclaredAccessibility == Accessibility.Internal)))
            .ToList();

        var baseTypeSymbol = namedTypeSymbol.BaseType;

        while (baseTypeSymbol != null)
        {
            symbols.AddRange(GetPropertySymbols(baseTypeSymbol, includeInternals));
            baseTypeSymbol = baseTypeSymbol.BaseType;
        }

        return symbols;
    }
}

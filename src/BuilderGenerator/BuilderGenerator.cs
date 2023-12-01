using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading;
using BuilderGenerator.Diagnostics;
using BuilderGenerator.Generation;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace BuilderGenerator;

[Generator]
internal class BuilderGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var classDeclarations = context.SyntaxProvider.CreateSyntaxProvider(WhereTypeHasAttributes, GetTypesWithBuilderForAttribute).Where(static node => node is not null);
        var compilationAndClasses = context.CompilationProvider.Combine(classDeclarations.Collect());
        context.RegisterSourceOutput(compilationAndClasses, static (spc, source) => Execute(source.Item1, source.Item2!, spc));

        context.RegisterPostInitializationOutput(
            x =>
            {
                // Inject base classes that never change
                x.AddSource("BuilderBaseClass", SourceText.From(EmbeddedResourceProvider.GetResourceByName("Templates.BuilderBaseClass.txt"), Encoding.UTF8));
                x.AddSource("BuilderForAttribute", SourceText.From(EmbeddedResourceProvider.GetResourceByName("Templates.BuilderForAttribute.txt"), Encoding.UTF8));
            });
    }

    private static void Execute(Compilation compilation, ImmutableArray<TypeDeclarationSyntax> classes, SourceProductionContext context)
    {
        if (classes.IsDefaultOrEmpty)
        {
            return;
        }

        var distinctClasses = classes.Distinct();

        foreach (var typeDeclaration in distinctClasses)
        {
            try
            {
                var semanticModel = compilation.GetSemanticModel(typeDeclaration.SyntaxTree);
                var typeSymbol = semanticModel.GetDeclaredSymbol(typeDeclaration, context.CancellationToken);
                var templateParser = new TemplateParser();

                if (typeSymbol is not INamedTypeSymbol namedTypeSymbol)
                {
                    continue;
                }

                if (namedTypeSymbol.IsAbstract)
                {
                    context.ReportDiagnostic(DiagnosticDescriptors.TargetClassIsAbstract(typeDeclaration.GetLocation(), typeDeclaration.Identifier.ToString()));
                }

                var attributeSymbol = namedTypeSymbol.GetAttributes().SingleOrDefault(x => x.AttributeClass!.Name == "BuilderForAttribute");

                if (attributeSymbol is null)
                {
                    continue;
                }

                var includeInternals = (bool)attributeSymbol.ConstructorArguments[1].Value!;

                var classDescriptor = new ClassDescriptor(attributeSymbol.ConstructorArguments[0]);

                var targetClassProperties = classDescriptor.GetProperties(includeInternals);
                var builderClassName = typeSymbol.Name;

                templateParser.SetTag("GeneratedAt", DateTime.Now.ToString("s"));
                templateParser.SetTag("BuilderClassUsingBlock", ((CompilationUnitSyntax)typeDeclaration.SyntaxTree.GetRoot()).Usings.ToString());
                templateParser.SetTag("BuilderClassNamespace", typeSymbol.ContainingNamespace.ToString());
                templateParser.SetTag("BuilderClassAccessibility", typeSymbol.DeclaredAccessibility.ToString().ToLower());
                templateParser.SetTag("BuilderClassName", builderClassName);
                templateParser.SetTag("TargetClassName", classDescriptor.Name);
                templateParser.SetTag("TargetClassFullName", classDescriptor.FullName);

                var properties = GenerateProperties(templateParser, targetClassProperties);
                var buildMethod = GenerateBuildMethod(templateParser, targetClassProperties, classDescriptor.IsRecord);
                var withMethods = GenerateWithMethods(templateParser, targetClassProperties);
                var valueFormMethod = GenerateWithValuesFromMethod(templateParser, targetClassProperties);

                templateParser.SetTag("Properties", properties);
                templateParser.SetTag("BuildMethod", buildMethod);
                templateParser.SetTag("WithMethods", withMethods);
                templateParser.SetTag("WithValueFromMethod", valueFormMethod);

                var source = templateParser.ParseString(EmbeddedResourceProvider.GetResourceByName("Templates.BuilderClass.txt"));

                context.AddSource($"{builderClassName}.generated.cs", SourceText.From(source, Encoding.UTF8));
            }
            catch (Exception e)
            {
                context.ReportDiagnostic(DiagnosticDescriptors.UnexpectedErrorDiagnostic(e, typeDeclaration.GetLocation(), typeDeclaration.Identifier.ToString()));
            }
        }
    }

    private static string GenerateBuildMethod(TemplateParser templateParser, List<ClassProperty> properties, bool isRecord)
    {
        var classProperties = properties.ToArray();

        if (isRecord)
        {
            var parameters = string.Join(
                Environment.NewLine,
                classProperties.Select(
                    (x, i) =>
                    {
                        templateParser.SetTag("PropertyName", x.Name);

                        var value = templateParser.ParseString(EmbeddedResourceProvider.GetResourceByName("Templates.BuildMethodConstructorParameter.txt"));

                        if (i == classProperties.Length - 1)
                        {
                            value = value.TrimEnd(',');
                        }

                        return value;
                    }));

            templateParser.SetTag("Parameters", parameters);
            var result = templateParser.ParseString(EmbeddedResourceProvider.GetResourceByName("Templates.BuildMethodConstructor.txt"));

            return result;
        }
        else
        {
            var setters = string.Join(
                Environment.NewLine,
                classProperties.Select(
                    x =>
                    {
                        templateParser.SetTag("PropertyName", x.Name);

                        return templateParser.ParseString(EmbeddedResourceProvider.GetResourceByName("Templates.BuildMethodSetter.txt"));
                    }));

            templateParser.SetTag("Setters", setters);
            var result = templateParser.ParseString(EmbeddedResourceProvider.GetResourceByName("Templates.BuildMethodInitializer.txt"));

            return result;
        }
    }

    private static string GenerateProperties(TemplateParser templateParser, List<ClassProperty> properties)
    {
        var result = string.Join(
            Environment.NewLine,
            properties.Select(
                x =>
                {
                    templateParser.SetTag("PropertyName", x.Name);
                    templateParser.SetTag("PropertyType", x.TypeName);

                    return templateParser.ParseString(EmbeddedResourceProvider.GetResourceByName("Templates.PropertyDeclaration.txt"));
                }));

        return result;
    }

    private static string GenerateWithMethods(TemplateParser templateParser, List<ClassProperty> properties)
    {
        var result = string.Join(
            Environment.NewLine,
            properties.Select(
                x =>
                {
                    templateParser.SetTag("PropertyName", x.Name);
                    templateParser.SetTag("PropertyType", x.TypeName);

                    return templateParser.ParseString(EmbeddedResourceProvider.GetResourceByName("Templates.WithMethod.txt"));
                }));

        return result;
    }

    private static string GenerateWithValuesFromMethod(TemplateParser templateParser, List<ClassProperty> properties)
    {
        var withMethodCalls = string.Join(
            Environment.NewLine,
            properties.Select(
                x =>
                {
                    templateParser.SetTag("PropertyName", x.Name);

                    return templateParser.ParseString(EmbeddedResourceProvider.GetResourceByName("Templates.WithValuesFromMethodInner.txt"));
                }));

        templateParser.SetTag("WithMethods", withMethodCalls);
        var result = templateParser.ParseString(EmbeddedResourceProvider.GetResourceByName("Templates.WithValuesFromMethodOuter.txt"));

        return result;
    }

    private static bool WhereTypeHasAttributes(SyntaxNode node, CancellationToken _)
        => node is TypeDeclarationSyntax { AttributeLists.Count: > 0 };

    private static TypeDeclarationSyntax? GetTypesWithBuilderForAttribute(GeneratorSyntaxContext context, CancellationToken token)
    {
        var node = context.Node;

        if (node is not TypeDeclarationSyntax typeNode)
        {
            return null;
        }

        var model = context.SemanticModel;

        var typeSymbol = model.GetDeclaredSymbol(typeNode, token);

        if (typeSymbol is not INamedTypeSymbol namedTypeSymbol)
        {
            return null;
        }

        return namedTypeSymbol.GetAttributes().Any(x => x.AttributeClass?.Name == "BuilderForAttribute")
            ? typeNode
            : null;
    }
}

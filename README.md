[![Nuget](https://img.shields.io/nuget/dt/kaede)](https://www.nuget.org/packages/BuilderGenerator/)
[![GitHub](https://img.shields.io/github/license/am4u/kaede)](https://opensource.org/licenses/MIT)
[![Build status](https://ci.appveyor.com/api/projects/status/ioq5c465m65hjja2?svg=true)](https://ci.appveyor.com/project/am4u/kaede)
[![GitHub issues](https://img.shields.io/github/issues/melgrubb/buildergenerator)](https://github.com/MelGrubb/BuilderGenerator/issues)

# BuilderGenerator2 - an updated version of BuilderGenerator #

An updated version of BuilderGenerator - a .NET Source Generator by MelGrubb, that is designed to add "Builders" to your projects. [Builders](https://en.wikipedia.org/wiki/Builder_pattern) are an object creation pattern, similar to the [Object Mother](https://martinfowler.com/bliki/ObjectMother.html) pattern. Object Mothers and Builders are most commonly used to create objects for testing, but they can be used anywhere you want "canned" objects.

## What is new? ##

* Record type support - Immutable records now work with the Source Generator
* More soon

For more complete documentation, please see the [documentation site](https://melgrubb.github.io/BuilderGenerator/) or the raw [documentation source](https://github.com/MelGrubb/BuilderGenerator/blob/main/docs/index.md).

## Installation ##

Kaede is installed as an analyzer via a NuGet package - for more info on how to install/add this package to your solution: https://www.nuget.org/packages/Kaede/

## Usage ##

After the package has been installed into your project:

1. Create a new partial class that will hold your builder methods.
2. Decorate it with the ```BuilderFor``` attribute, specifying the type of class that the builder is meant to build. For example: 
```csharp
[BuilderFor(typeof(Foo))]
public partial class FooBuilder
{
}
``` 
3. Rebuild your project. The source generator will run and autogenerate methods in a separate partial class file, for each property in the type you specified in Step 2. 

You can also add factory methods to your partial class which can craft specific data scenarios: 

```csharp
[BuilderFor(typeof(Amogus))]
public partial class AmogusBuilder
{
    public static AmogusBuilder IsSus()
    {
        return new AmogusBuilder()
            .WithIsSus(true);
    }
    
    public static AmogusBuilder IsNotSus()
    {
        return new AmogusBuilder()
            .WithIsSus(false);
    }
}
``` 

## Version History ##
- v1.0
  - Initial fork
  - Updated to include record type support

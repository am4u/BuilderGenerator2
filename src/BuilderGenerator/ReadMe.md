A fork of BuilderGenerator, a .NET Source Generator that is designed to add fluent builder classes to your projects. Save time by automating the builder pattern.

## What are Builders?
[Builders](https://en.wikipedia.org/wiki/Builder_pattern) are an object creation pattern, similar to the [Object Mother](https://martinfowler.com/bliki/ObjectMother.html) pattern. Object Mothers and Builders are most commonly used to create objects for testing, but they can be used anywhere you want "canned" objects. 

## Example Usage ##

1. Create a new partial class that will hold your builder methods - for this example, we'll call it ```FooBuilder```, as the class we want to work with is called `Foo`. 
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
   [BuilderFor(typeof(Foo))]
   public partial class FooBuilder
   {
       public static FooBuilder Bar()
       {
           return new FooBuilder()
               .WithBar(true);
       }
       
       public static FooBuilder NotBar()
       {
           return new FooBuilder()
               .WithBar(false);
       }
   }
   ```

## Features
### Record types
With this fork, you can use the `[BuilderFor]` attribute on C# record types:
```csharp
public record MyRecord(bool Hello);

[BuilderFor(typeof(MyRecord)]
public partial class MyRecordBuilder
{
}

var builder = new MyRecordBuilder().WithHello(true).Build();
```

### WithValuesFrom
The new `WithValuesFrom` method allows you to copy values from an existing instance into your builder:
```csharp
new MyBuilder().WithValuesFrom(anotherObject)
```

## Documentation & Source Code
For more information, please visit the [GitHub repository](https://github.com/safalin1/BuilderGenerator2).

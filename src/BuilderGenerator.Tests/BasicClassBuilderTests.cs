using Bogus;
using BuilderGenerator.Tests.Builders;
using BuilderGenerator.Tests.Models;
using FluentAssertions;
using Xunit;

namespace BuilderGenerator.Tests;

public class BasicClassBuilderTests
{
    private readonly Faker _faker = new();

    [Fact]
    public void Builder_WhenConstructed_ShouldHaveDefaults()
    {
        // Arrange
        var builder = new BasicClassBuilder();
        var expected = new BasicClass();

        // Act
        var actual = builder.Build();

        // Assert
        actual.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void Build_WhenProvidedValues_ShouldHaveExpectedValues()
    {
        // Arrange
        var one = _faker.Random.Int();
        var two = _faker.Random.Bool();
        var three = _faker.Random.String();

        var builder = new BasicClassBuilder()
            .WithProperty1(one)
            .WithProperty2(two)
            .WithProperty3(three);

        var expected = new BasicClass
        {
            Property1 = one,
            Property2 = two,
            Property3 = three
        };

        // Act
        var actual = builder.Build();

        // Assert
        actual.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void Build_WhenProvidedOtherInstance_ShouldHaveExpectedValues()
    {
        // Arrange
        var one = _faker.Random.Int();
        var two = _faker.Random.Bool();
        var three = _faker.Random.String();

        var otherInstance = new BasicClassBuilder()
            .WithProperty1(one)
            .WithProperty2(two)
            .WithProperty3(three)
            .Build();

        var threeUpdated = _faker.Random.String();

        var builder = new BasicClassBuilder()
            .WithValuesFrom(otherInstance)
            .WithProperty3(threeUpdated);

        var expected = new BasicClass
        {
            Property1 = one,
            Property2 = two,
            Property3 = threeUpdated
        };

        // Act
        var actual = builder.Build();

        // Assert
        actual.Should().BeEquivalentTo(expected);
    }
}

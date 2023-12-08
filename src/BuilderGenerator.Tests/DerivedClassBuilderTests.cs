using Bogus;
using BuilderGenerator.Tests.Builders;
using BuilderGenerator.Tests.Models;
using FluentAssertions;
using Xunit;

namespace BuilderGenerator.Tests;

public class DerivedClassBuilderTests
{
    private readonly Faker _faker = new();

    [Fact]
    public void Builder_WhenConstructed_ShouldHaveDefaults()
    {
        // Arrange
        var builder = new DerivedClassBuilder();
        var expected = new DerivedClass();

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
        var four = _faker.Random.Guid();

        var builder = new DerivedClassBuilder()
            .WithProperty1(one)
            .WithProperty2(two)
            .WithProperty3(three)
            .WithProperty4(four);

        var expected = new DerivedClass
        {
            Property1 = one,
            Property2 = two,
            Property3 = three,
            Property4 = four
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
        var four = _faker.Random.Guid();
        var six = _faker.Random.Int();

        var otherInstance = new DerivedClassBuilder()
            .WithProperty1(one)
            .WithProperty2(two)
            .WithProperty3(three)
            .WithProperty4(four)
            .WithProperty6(six)
            .Build();

        var threeUpdated = _faker.Random.String();

        var builder = new DerivedClassBuilder()
            .WithValuesFrom(otherInstance)
            .WithProperty3(threeUpdated);

        var expected = new DerivedClass
        {
            Property1 = one,
            Property2 = two,
            Property3 = threeUpdated,
            Property4 = four,
            Property6 = six
        };

        // Act
        var actual = builder.Build();

        // Assert
        actual.Should().BeEquivalentTo(expected);
    }
}

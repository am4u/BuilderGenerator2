using Bogus;
using BuilderGenerator.Tests.Builders;
using BuilderGenerator.Tests.Models;
using FluentAssertions;
using Xunit;

namespace BuilderGenerator.Tests;

public class SecondLevelDerivedClassBuilderTests
{
    private readonly Faker _faker = new();

    [Fact]
    public void Builder_WhenConstructed_ShouldHaveDefaults()
    {
        // Arrange
        var builder = new SecondLevelDerivedClassBuilder();
        var expected = new SecondLevelDerivedClass();

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
        var five = _faker.Internet.IpAddress();
        var six = _faker.Random.Bool();

        var builder = new SecondLevelDerivedClassBuilder()
            .WithProperty1(one)
            .WithProperty2(two)
            .WithProperty3(three)
            .WithProperty5(five)
            .WithProperty6(six);

        var expected = new SecondLevelDerivedClass
        {
            Property1 = one,
            Property2 = two,
            Property3 = three,
            Property5 = five,
            Property6 = six
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
        var five = _faker.Internet.IpAddress();
        var six = _faker.Random.Bool();

        var otherInstance = new SecondLevelDerivedClassBuilder()
            .WithProperty1(one)
            .WithProperty2(two)
            .WithProperty3(three)
            .WithProperty5(five)
            .WithProperty6(six)
            .Build();

        var threeUpdated = _faker.Random.String();

        var builder = new SecondLevelDerivedClassBuilder()
            .WithValuesFrom(otherInstance)
            .WithProperty3(threeUpdated);

        var expected = new SecondLevelDerivedClass
        {
            Property1 = one,
            Property2 = two,
            Property3 = threeUpdated,
            Property5 = five,
            Property6 = six
        };

        // Act
        var actual = builder.Build();

        // Assert
        actual.Should().BeEquivalentTo(expected);
    }
}

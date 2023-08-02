using Bogus;
using BuilderGenerator.Tests.Builders;
using BuilderGenerator.Tests.Models;
using FluentAssertions;
using Xunit;

namespace BuilderGenerator.Tests;

public class BasicRecordBuilderTests
{
    private readonly Faker _faker;

    public BasicRecordBuilderTests()
    {
        _faker = new Faker();
    }

    [Fact]
    public void Builder_WhenConstructed_ShouldHaveDefaults()
    {
        // Arrange
        var builder = new BasicRecordBuilder();

        var expected = new BasicRecord(
            ParameterOne: 0,
            ParameterTwo: false,
            ParameterThree: null);

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

        var builder = new BasicRecordBuilder()
            .WithParameterOne(one)
            .WithParameterTwo(two)
            .WithParameterThree(three);

        var expected = new BasicRecord(
            ParameterOne: one,
            ParameterTwo: two,
            ParameterThree: three);

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

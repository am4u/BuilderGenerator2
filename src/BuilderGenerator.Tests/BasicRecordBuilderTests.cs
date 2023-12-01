using Bogus;
using BuilderGenerator.Tests.Builders;
using BuilderGenerator.Tests.Models;
using FluentAssertions;
using Xunit;

namespace BuilderGenerator.Tests;

public class BasicRecordBuilderTests
{
    private readonly Faker _faker = new();

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

        var otherInstance = new BasicRecordBuilder()
            .WithParameterOne(one)
            .WithParameterTwo(two)
            .WithParameterThree(three)
            .Build();

        var threeUpdated = _faker.Random.String();

        var builder = new BasicRecordBuilder()
            .WithValuesFrom(otherInstance)
            .WithParameterThree(threeUpdated);

        var expected = new BasicRecord(one, two, threeUpdated);

        // Act
        var actual = builder.Build();

        // Assert
        actual.Should().BeEquivalentTo(expected);
    }
}

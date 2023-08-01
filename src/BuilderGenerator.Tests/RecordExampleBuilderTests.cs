using BuilderGenerator.Tests.Builders;
using BuilderGenerator.Tests.Models;
using FluentAssertions;
using Xunit;

namespace BuilderGenerator.Tests;

public class UserRecordBuilderTests
{
    [Fact]
    public void Builder_WhenConstructed_ShouldHaveDefaults()
    {
        // Arrange
        var builder = new UserRecordBuilder();

        var expected = new UserRecord(
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
        var builder = new UserRecordBuilder()
            .WithParameterOne(123)
            .WithParameterTwo(true)
            .WithParameterThree("Three");

        var expected = new UserRecord(
            ParameterOne: 123,
            ParameterTwo: true,
            ParameterThree: "Three");

        // Act
        var actual = builder.Build();

        // Assert
        actual.Should().BeEquivalentTo(expected);
    }
}

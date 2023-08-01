using System;
using BuilderGenerator.Tests.Models;

namespace BuilderGenerator.Tests.Builders;

[BuilderFor(typeof(UserRecord))]
public partial class UserRecordBuilder
{
    public static UserRecordBuilder Typical()
    {
        return new UserRecordBuilder()
            .WithParameterOne(Random.Shared.Next())
            .WithParameterTwo(Random.Shared.Next(0, 2) == 1)
            .WithParameterThree(Guid.NewGuid().ToString);
    }
}

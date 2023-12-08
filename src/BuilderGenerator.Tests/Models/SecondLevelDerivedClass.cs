using System.Net;

namespace BuilderGenerator.Tests.Models;

public class SecondLevelDerivedClass : DerivedClass
{
    public IPAddress Property5 { get; set; }

    public new bool Property6 { get; set; }
}

// ReSharper disable UnusedMember.Global

namespace BuilderGenerator
{
    internal static class Templates
    {
        internal const string BuilderClass = @"{{BuilderClassUsingBlock}}
using System.CodeDom.Compiler;
#nullable enable

namespace {{BuilderClassNamespace}}
{
    // Generated at {{GeneratedAt}}
    {{BuilderClassAccessibility}} partial class {{BuilderClassName}} : BuilderGenerator.Builder<{{TargetClassFullName}}>
    {
{{Properties}}
{{BuildMethod}}
{{WithMethods}}
    }
}";

        internal const string BuildMethodSetter = "                        {{PropertyName}} = {{PropertyName}}.Value,";

        internal const string BuildMethod = @"
        public override {{TargetClassFullName}} Build()
        {
            if (Object?.IsValueCreated != true)
            {
                Object = new System.Lazy<{{TargetClassFullName}}>(() => 
                {
                    var result = new {{TargetClassFullName}} 
                    {
{{Setters}}
                    };

                    return result;
                });

                PostProcess(Object.Value);
            }

            return Object.Value;
        }";

        internal const string Property = @"        public System.Lazy<{{PropertyType}}> {{PropertyName}} = new System.Lazy<{{PropertyType}}>(() => default({{PropertyType}}));";

        internal const string WithMethod = @"
        public {{BuilderClassName}} With{{PropertyName}}({{PropertyType}} value)
        {
            return With{{PropertyName}}(() => value);
        }

        public {{BuilderClassName}} With{{PropertyName}}(System.Func<{{PropertyType}}> func)
        {
            {{PropertyName}} = new System.Lazy<{{PropertyType}}>(func);
            return this;
        }

        public {{BuilderClassName}} Without{{PropertyName}}()
        {                    
            {{PropertyName}} = new System.Lazy<{{PropertyType}}>(() => default({{PropertyType}}));
            return this;
        }";

        internal const string BuilderForAttribute = @"namespace BuilderGenerator
{
    [System.AttributeUsage(System.AttributeTargets.Class)]
    public class BuilderForAttribute : System.Attribute
    {
        public System.Type Type { get; }

        public BuilderForAttribute(System.Type type)
        {
            this.Type = type;
        }
    }
}";

        internal const string BuilderBaseClass = @"#nullable disable

namespace BuilderGenerator
{
    /// <summary>Base class for object builder classes.</summary>
    /// <typeparam name=""T"">The type of the objects built by this builder.</typeparam>
    public abstract class Builder<T> where T : class
    {
        /// <summary>Gets or sets the object returned by this builder.</summary>
        /// <value>The constructed object.</value>
        #pragma warning disable CA1720 // Identifier contains type name
        protected System.Lazy<T>? Object { get; set; }
        #pragma warning restore CA1720 // Identifier contains type name

        /// <summary>Builds the object instance.</summary>
        /// <returns>The constructed object.</returns>
        public abstract T Build();

        protected virtual void PostProcess(T value)
        {
        }

        /// <summary>Sets the object to be returned by this instance.</summary>
        /// <param name=""value"">The object to be returned.</param>
        /// <returns>A reference to this builder instance.</returns>
        public Builder<T> WithObject(T value)
        {
            Object = new System.Lazy<T>(value);

            return this;
        }
    }
}";
    }
}

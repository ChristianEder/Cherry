using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using Cherry.Expressions.Builders;
using Microsoft.CSharp;

namespace Cherry.Expressions
{
    public static class ExpressionStringifierExtensions
    {
        public static string ToCSharpString(this Expression expression)
        {
            var state = new ExpressionStringBuilderState();
            state.Add(expression, "expression");
            return state.ToString();
        }

        public static T Compile<T>(string csharpString)
            where T : Expression
        {
            var csc = new CSharpCodeProvider(new Dictionary<string, string>() { { "CompilerVersion", "v4.0" } });
            var parameters = new CompilerParameters(new[] { "mscorlib.dll", "System.Core.dll" }, Guid.NewGuid() + ".dll", false);
            parameters.GenerateExecutable = false;
            var sources = string.Format(@"using System.Linq;
              using System.Linq.Expressions;
            public class MyExpression {{
              public static Expression Build() {{
                {0}
                return expression;
              }}
            }}", csharpString);
            CompilerResults results = csc.CompileAssemblyFromSource(parameters,sources);

            if (results.Errors.HasErrors)
            {
                throw new InvalidOperationException("Something went wrong");
            }

            var expression = results.CompiledAssembly.GetType("MyExpression")
                .GetMethod("Build", BindingFlags.Public | BindingFlags.Static)
                .Invoke(null, new object[0]);

            return (T)expression;

        }
    }
}
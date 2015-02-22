using System;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Cherry.Expressions.Spec
{
    [TestClass]
    public class ExpressionBuilderSpec
    {
        [TestMethod]
        public void Test()
        {
         //var ex = MyExpression.Build();

            var numbers = new[] { 1, 4, 6, 7 }.AsQueryable();
            var otherNumbers = new[] { 1, 4, 6, 7 }.AsQueryable();

            TestExpression(() => 3);
            TestExpression(() => numbers.Sum(n => n < 5 && true ? (n + 1) : n), "var numbers = new[] { 1, 4, 6, 7 }.AsQueryable();");
        }

        private void TestExpression<T>(Expression<Func<T>> expected, params string[] context)
        {
            var csharp = expected.ToCSharpString();

            csharp = string.Join(Environment.NewLine, context) + Environment.NewLine + csharp;
            var actual = ExpressionStringifierExtensions.Compile<Expression<Func<T>>>(csharp);

            Assert.IsNotNull(actual);

            var expectedValue = expected.Compile()();
            var actualValue = actual.Compile()();

            Assert.AreEqual(expectedValue, actualValue);
        }
    }
}

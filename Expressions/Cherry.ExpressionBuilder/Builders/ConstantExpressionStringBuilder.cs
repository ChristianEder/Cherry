using System;
using System.Linq.Expressions;

namespace Cherry.Expressions.Builders
{
    internal class ConstantExpressionStringBuilder : ExpressionStringBuilder<ConstantExpression>
    {
        public override string Build(ConstantExpression expression, string variableName, ExpressionStringBuilderState state)
        {
            return string.Format("Expression.Constant({0}, typeof({1}))",
                expression.Type == typeof(Boolean)
                ? expression.Value.ToString().ToLowerInvariant()
                : expression.Type == typeof(String)
                    ? "\"" + expression.Value + "\""
                    : expression.Type == typeof(char)
                        ? "'" + expression.Value + "'"
                        : expression.Value, expression.Type);
        }
    }
}
using System;
using System.Linq.Expressions;

namespace Cherry.Expressions.Builders
{
    internal class UnaryExpressionStringBuilder : ExpressionStringBuilder<UnaryExpression>
    {
        public override string Build(UnaryExpression expression, string variableName, ExpressionStringBuilderState state)
        {
            if (expression.NodeType == ExpressionType.Quote)
            {
                var name = state.Add(expression.Operand, variableName + "_Quote");
                return string.Format("Expression.Quote({0})", name);
            }

            throw new InvalidOperationException("Invalid UnaryExpression");
        }
    }
}
using System.Linq.Expressions;

namespace Cherry.Expressions.Builders
{
    internal class ConditionalExpressionStringBuilder : ExpressionStringBuilder<ConditionalExpression>
    {
        public override string Build(ConditionalExpression expression, string variableName, ExpressionStringBuilderState state)
        {
            var condition = state.Add(expression.Test, variableName + "_Condition");
            var ifTrue = state.Add(expression.IfTrue, variableName + "_IfTrue");
            var ifFalse = state.Add(expression.IfFalse, variableName + "_IfFalse");

            return string.Format("Expression.Condition({0}, {1}, {2}, typeof({3}))", condition, ifTrue, ifFalse, TypeString(expression.Type));
        }
    }
}
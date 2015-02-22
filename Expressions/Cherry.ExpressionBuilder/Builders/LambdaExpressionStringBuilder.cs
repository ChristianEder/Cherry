using System.Linq.Expressions;

namespace Cherry.Expressions.Builders
{
    internal class LambdaExpressionStringBuilder : ExpressionStringBuilder<LambdaExpression>
    {
        public override string Build(LambdaExpression expression, string variableName, ExpressionStringBuilderState state)
        {
            var bodyName = state.Add(expression.Body, variableName + "_Body");
            var parameters = Add(state, expression.Parameters, i => variableName + "_P" + i);
            return string.Format("Expression.Lambda({0}{1})", bodyName, JoinAsParameters(parameters));
        }
    }
}
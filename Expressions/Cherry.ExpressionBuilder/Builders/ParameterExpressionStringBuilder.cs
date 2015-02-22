using System.Linq.Expressions;

namespace Cherry.Expressions.Builders
{
    internal class ParameterExpressionStringBuilder : ExpressionStringBuilder<ParameterExpression>
    {
        public override string Build(ParameterExpression expression, string variableName, ExpressionStringBuilderState state)
        {
            return string.Format("Expression.Parameter(typeof({0}), \"{1}\")", TypeString(expression.Type), expression.Name);
        }
    }
}
using System.Linq.Expressions;

namespace Cherry.Expressions.Builders
{
    internal class BinaryExpressionStringBuilder : ExpressionStringBuilder<BinaryExpression>
    {
        public override string Build(BinaryExpression expression, string variableName, ExpressionStringBuilderState state)
        {
            var left = state.Add(expression.Left, variableName + "_Left");
            var right = state.Add(expression.Right, variableName + "_Right");
          
            return string.Format("Expression.{0}({1}, {2})", expression.NodeType, left, right);
        }
    }
}
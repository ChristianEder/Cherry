using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Cherry.Expressions.Builders
{
    internal class ExpressionStringBuilderState
    {
        private readonly Dictionary<Expression, string> _added = new Dictionary<Expression, string>();
        private readonly List<string> _builder = new List<string>();

        public string Add(Expression expression, string variableName)
        {
            string name;
            if (_added.TryGetValue(expression, out name))
            {
                return name;
            }

            string code = null;

            if (expression is LambdaExpression)
            {
                code = new LambdaExpressionStringBuilder().Build((LambdaExpression)expression, variableName, this);
            }
            else if (expression is ConstantExpression)
            {
                code = new ConstantExpressionStringBuilder().Build((ConstantExpression)expression, variableName, this);
            }
            else if (expression is ParameterExpression)
            {
                code = new ParameterExpressionStringBuilder().Build((ParameterExpression)expression, variableName, this);
            }
            else if (expression is MethodCallExpression)
            {
                code = new MethodCallExpressionStringBuilder().Build((MethodCallExpression)expression, variableName, this);
            }
            else if (expression is MemberExpression)
            {
                code = new MemberExpressionStringBuilder().Build((MemberExpression)expression, variableName, this);
            }
            else if (expression is UnaryExpression)
            {
                code = new UnaryExpressionStringBuilder().Build((UnaryExpression)expression, variableName, this);
            }
            else if (expression is BinaryExpression)
            {
                code = new BinaryExpressionStringBuilder().Build((BinaryExpression)expression, variableName, this);
            }
            else if (expression is ConditionalExpression)
            {
                code = new ConditionalExpressionStringBuilder().Build((ConditionalExpression)expression, variableName, this);
            }
            else
            {
                throw new InvalidOperationException("Unknown expression type");
            }

            if (!string.IsNullOrEmpty(code))
            {
                _builder.Add(string.Format("var {0} = {1};", variableName, code));
            }

            _added.Add(expression, variableName);
            return variableName;
        }

        public void AddCode(string code)
        {
            _builder.Add(code);
        }

        public override string ToString()
        {
            return string.Join(Environment.NewLine, _builder);
        }
    }
}
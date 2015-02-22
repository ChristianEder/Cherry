using System.Linq;
using System.Linq.Expressions;

namespace Cherry.Expressions.Builders
{
    internal class MethodCallExpressionStringBuilder : ExpressionStringBuilder<MethodCallExpression>
    {
        public override string Build(MethodCallExpression expression, string variableName, ExpressionStringBuilderState state)
        {
            var arguments = Add(state, expression.Arguments, i => variableName + "_A" + i);
            
            var objectName = expression.Object == null ? "null" : variableName + "_Object";
            if (expression.Object != null)
            {
                objectName = state.Add(expression.Object, objectName);
            }

            var m = expression.Method;

            var makeGeneric = "";
            if (m.IsGenericMethod)
            {
                var typeArguments = string.Join(",", m.GetGenericArguments().Select(t => "typeof(" + TypeString(t)+ ")"));
                makeGeneric = ".Select(tm => tm.MakeGenericMethod(" + typeArguments + "))";
            }

            string methodInfo = string.Format("typeof({0}).GetMethods().Where(tm => tm.Name == \"{1}\").Where({2}){3}.Where({4}).First()",
                TypeString(m.DeclaringType),
                m.Name,
                "tm => tm.GetParameters().Length == " + m.GetParameters().Length,
                makeGeneric,
                "tm => tm.ReturnType == typeof(" + TypeString(m.ReturnType) + ")");

            return string.Format("Expression.Call({0}, {1}{2})", objectName, methodInfo, JoinAsParameters(arguments));
        }
    }
}
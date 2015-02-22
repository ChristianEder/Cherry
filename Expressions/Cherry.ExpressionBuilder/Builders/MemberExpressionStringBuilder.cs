using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Cherry.Expressions.Builders
{
    internal class MemberExpressionStringBuilder : ExpressionStringBuilder<MemberExpression>
    {
        public override string Build(MemberExpression expression, string variableName, ExpressionStringBuilderState state)
        {
            var member = expression.Member;
            if (member.DeclaringType.Name.Contains("__DisplayClass"))
            {
                Type memberType;
                if (member is FieldInfo)
                {
                    memberType = ((FieldInfo)member).FieldType;
                }
                else if (member is PropertyInfo)
                {
                    memberType = ((PropertyInfo) member).PropertyType;
                }
                else
                {
                    throw new InvalidOperationException();
                }

                return string.Format("Expression.Constant({0}, typeof({1}))", member.Name, TypeString(memberType));
            }

            var ofMember = state.Add(expression.Expression, variableName + "_" + member.Name);

            string memberInfo = string.Format("typeof({0}).GetMembers().Where(tm => tm.Name == \"{1}\").First()",
              TypeString(member.DeclaringType),
              member.Name);

            return string.Format("Expression.MakeMemberAccess({0}, {1})", ofMember, memberInfo);
        }
    }
}
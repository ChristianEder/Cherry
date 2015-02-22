using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Cherry.Expressions.Builders
{
    internal abstract class ExpressionStringBuilder<T> where T : Expression
    {
        public abstract string Build(T expression, string variableName, ExpressionStringBuilderState state);

        protected List<string> Add(ExpressionStringBuilderState state, IEnumerable<Expression> expressions, Func<int, string> name)
        {
            var names = new List<string>();
            for (int index = 0; index < expressions.Count(); index++)
            {
                var parameter = expressions.ElementAt(index);
                var parameterName = name(index);
                parameterName = state.Add(parameter, parameterName);
                names.Add(parameterName);
            }
            return names;
        }

        protected string JoinAsParameters(List<string> names)
        {
            if (names.Any())
            {
                return ", " + string.Join(", ", names);
            }
            return "";
        }

        protected string TypeString(Type type)
        {
            StringBuilder retType = new StringBuilder();

            if (type.IsGenericTypeDefinition)
            {
                var typeString = type.Name + "<" + string.Join(",", type.GetGenericArguments().Select(s => string.Empty)) + ">";
                return typeString;
            }
            if (type.IsGenericType)
            {
                var name = type.FullName ?? type.Name;
                string[] parentType = name.Split('`');
                // We will build the type here.
                Type[] arguments = type.GetGenericArguments();

                StringBuilder argList = new StringBuilder();
                foreach (Type t in arguments)
                {
                    // Let's make sure we get the argument list.
                    string arg = TypeString(t);
                    if (argList.Length > 0)
                    {
                        argList.AppendFormat(", {0}", arg);
                    }
                    else
                    {
                        argList.Append(arg);
                    }
                }

                if (argList.Length > 0)
                {
                    retType.AppendFormat("{0}<{1}>", parentType[0], argList.ToString());
                }
            }
            else
            {
                return type.ToString();
            }

            return retType.ToString();
        }
    }
}
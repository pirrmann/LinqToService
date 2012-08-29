using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using PeopleFinder;

namespace Tests.Tools
{
    public static class LambdaParser
    {
        private static readonly IDictionary<string, ExpressionType> CompareTypes;
        private static readonly IDictionary<string, MethodInfo> StringTestMethods;

        static LambdaParser()
        {
            CompareTypes = new Tuple<string, ExpressionType>[]
            {
                Tuple.Create(">=", ExpressionType.GreaterThanOrEqual),
                Tuple.Create(">", ExpressionType.GreaterThan),
                Tuple.Create("<=", ExpressionType.LessThanOrEqual),
                Tuple.Create("<", ExpressionType.LessThan),
                Tuple.Create("==", ExpressionType.Equal),
                Tuple.Create("!=", ExpressionType.NotEqual)
            }.ToDictionary(t => t.Item1, t => t.Item2);

            StringTestMethods = new Tuple<string, MethodInfo>[]
            {
                Tuple.Create("StartsWith", typeof(string).GetMethod("StartsWith", new[] { typeof(string) })),
                Tuple.Create("Contains", typeof(string).GetMethod("Contains", new[] { typeof(string) }))
            }.ToDictionary(t => t.Item1, t => t.Item2);
        }

        public static Expression<Func<Person, bool>> ParseWhere(string where)
        {
            ParameterExpression parameterExpression = Expression.Parameter(typeof(Person), "p");

            if (where == string.Empty || where == "true" || where == "false")
            {
                Expression body = Expression.Constant(where != "false", typeof(bool));
                return Expression.Lambda<Func<Person, bool>>(body, parameterExpression);
            }

            Regex r = new Regex("(.*) (==|<=|>=|>|<|!=) (.*)");
            Match m = r.Match(where);
            if (m.Success)
            {
                string propertyName = m.Groups[1].Value;
                string compareOperator = m.Groups[2].Value;
                string value = m.Groups[3].Value;

                PropertyInfo property = typeof(Person).GetProperty(propertyName);
                if (property == null) throw new Exception(string.Format("Property {0} not found on type Person", propertyName));

                Type memberType = property.PropertyType;
                object typedValue = Convert.ChangeType(value, memberType, CultureInfo.InvariantCulture);

                Expression body = Expression.MakeBinary(
                    CompareTypes[compareOperator],
                    Expression.Property(parameterExpression, property),
                    Expression.Constant(typedValue, memberType));

                return Expression.Lambda<Func<Person, bool>>(body, parameterExpression);
            }

            r = new Regex(@"(.*)\.(StartsWith|Contains)\(""(.*)""\)");
            m = r.Match(where);
            if (m.Success)
            {
                string propertyName = m.Groups[1].Value;
                string testMethod = m.Groups[2].Value;
                string value = m.Groups[3].Value;

                PropertyInfo property = typeof(Person).GetProperty(propertyName);
                if (property == null) throw new Exception(string.Format("Property {0} not found on type Person", propertyName));

                if (property.PropertyType != typeof(string)) throw new Exception(string.Format("Property {0} is not of type String", propertyName));
                object typedValue = Convert.ChangeType(value, typeof(string), CultureInfo.InvariantCulture);

                Expression body = Expression.Call(
                    Expression.Property(parameterExpression, property),
                    StringTestMethods[testMethod],
                    Expression.Constant(typedValue, typeof(string)));

                return Expression.Lambda<Func<Person, bool>>(body, parameterExpression);
            }

            throw new Exception("The method was unable to parse the where clause.");
        }
    }
}

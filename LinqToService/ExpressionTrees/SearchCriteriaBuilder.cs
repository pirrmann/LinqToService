using System;
using System.Linq.Expressions;
using PeopleFinder;
using PeopleFinder.Criteria;

namespace LinqToService.ExpressionTrees
{
    internal class SearchCriteriaBuilder : ExpressionVisitor
    {
        private SearchCriteria criteria;

        public SearchCriteria SearchCriteria { get { return this.criteria; } }

        internal SearchCriteriaBuilder()
        {
            this.criteria = new SearchCriteria();
        }

        internal Expression BuildCriteria(Expression expression)
        {
            return this.Visit(expression);
        }

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            if (node.Method.Name == "Take")
            {
                this.criteria = new SearchCriteria();
            }
            else if (node.Method.Name == "Where")
            {
                Type[] genericArguments = node.Method.GetGenericArguments();
                if (genericArguments.Length == 1 && genericArguments[0] == typeof(Person))
                {
                    return node.Update(
                        node.Object,
                        new Expression[]
                            {
                                Visit(node.Arguments[0]),
                                Expression.Quote(VisitWhereClause((LambdaExpression)((UnaryExpression)(node.Arguments[1])).Operand))
                            });

                }
            }

            return base.VisitMethodCall(node);
        }

        protected Expression VisitWhereClause(LambdaExpression whereExpression)
        {
            bool handled = false;

            Type memberType = null;
            string memberName = null;
            ExpressionType whereType = whereExpression.Body.NodeType;
            StringFilterType stringFilterType;

            switch (whereType)
            {
                case ExpressionType.Constant:
                    //TODO : handle these ?
                    break;

                case ExpressionType.IsFalse:
                case ExpressionType.IsTrue:
                    //TODO : handle these ?
                    break;

                case ExpressionType.Equal:
                case ExpressionType.GreaterThan:
                case ExpressionType.GreaterThanOrEqual:
                case ExpressionType.LessThan:
                case ExpressionType.LessThanOrEqual:
                case ExpressionType.NotEqual:
                    BinaryExpression binaryExpression = (BinaryExpression)whereExpression.Body;

                    if (binaryExpression.Left.NodeType == ExpressionType.MemberAccess)
                    {
                        MemberExpression memberExpression = (MemberExpression)binaryExpression.Left;
                        memberType = memberExpression.Type;
                        memberName = memberExpression.Member.Name;
                    }

                    if (memberName != null)
                    {
                        ConstantExpression comparedValueExpression = binaryExpression.Right as ConstantExpression;
                        object comparedValue = null;

                        if (comparedValueExpression != null)
                        {
                            comparedValue = comparedValueExpression.Value;
                        }

                        if (comparedValue != null)
                        {
                            switch (memberName)
                            {
                                case "Age":
                                    this.criteria.Age = new IntCriterion()
                                    {
                                        FilterType = ConvertExpressionTypeToNumericFilterType(whereType),
                                        Value = (int)comparedValue
                                    };
                                    handled = true;
                                    break;

                                case "FirstName":
                                    if (TryConvertExpressionTypeToStringFilterType(whereType, out stringFilterType))
                                    {
                                        this.criteria.FirstName = new StringCriterion()
                                        {
                                            FilterType = stringFilterType,
                                            Value = (string)comparedValue
                                        };
                                        handled = true;
                                    }
                                    break;

                                case "LastName":
                                    if (TryConvertExpressionTypeToStringFilterType(whereType, out stringFilterType))
                                    {
                                        this.criteria.LastName = new StringCriterion()
                                        {
                                            FilterType = stringFilterType,
                                            Value = (string)comparedValue
                                        };
                                        handled = true;
                                    }
                                    break;

                                default:
                                    break;
                            }
                        }
                    }
                    break;

                case ExpressionType.Call:
                    MethodCallExpression callExpression = (MethodCallExpression)whereExpression.Body;

                    if (callExpression.Object.NodeType == ExpressionType.MemberAccess)
                    {
                        MemberExpression memberExpression = (MemberExpression)callExpression.Object;
                        memberType = memberExpression.Type;
                        memberName = memberExpression.Member.Name;
                    }

                    if (memberName != null)
                    {
                        if (memberType == typeof(string) &&
                            (callExpression.Method.Name == "StartsWith" || callExpression.Method.Name == "Contains"))
                        {
                            ConstantExpression argumentExpression = callExpression.Arguments[0] as ConstantExpression;
                            object argumentValue = null;

                            if (argumentExpression != null)
                            {
                                argumentValue = argumentExpression.Value;
                            }

                            if (argumentValue != null)
                            {
                                switch (memberName)
                                {
                                    case "FirstName":
                                        if (TryConvertMethodCallToStringFilterType(callExpression.Method.Name, out stringFilterType))
                                        {
                                            this.criteria.FirstName = new StringCriterion()
                                            {
                                                FilterType = stringFilterType,
                                                Value = (string)argumentValue
                                            };
                                            handled = true;
                                        }
                                        break;

                                    case "LastName":
                                        if (TryConvertMethodCallToStringFilterType(callExpression.Method.Name, out stringFilterType))
                                        {
                                            this.criteria.LastName = new StringCriterion()
                                            {
                                                FilterType = stringFilterType,
                                                Value = (string)argumentValue
                                            };
                                            handled = true;
                                        }
                                        break;

                                    default:
                                        break;
                                }
                            }
                        }
                    }

                    break;
            }

            return handled
                ? Expression.Lambda<Func<Person, bool>>(Expression.Constant(true), whereExpression.Parameters[0])
                : whereExpression;
        }

        private NumericFilterType ConvertExpressionTypeToNumericFilterType(ExpressionType whereType)
        {
            switch (whereType)
            {
                case ExpressionType.Equal:
                    return NumericFilterType.Equals;
                case ExpressionType.GreaterThan:
                    return NumericFilterType.IsStrictlyGreaterThan;
                case ExpressionType.GreaterThanOrEqual:
                    return NumericFilterType.IsGreaterThan;
                case ExpressionType.LessThan:
                    return NumericFilterType.IsStrictlyLessThan;
                case ExpressionType.LessThanOrEqual:
                    return NumericFilterType.IsLessThan;
                case ExpressionType.NotEqual:
                    return NumericFilterType.NotEquals;
                default:
                    throw new ArgumentOutOfRangeException("whereType");
            }
        }

        private bool TryConvertExpressionTypeToStringFilterType(ExpressionType whereType, out StringFilterType stringFilterType)
        {
            switch (whereType)
            {
                case ExpressionType.Equal:
                    stringFilterType = StringFilterType.Equals;
                    return true;

                case ExpressionType.NotEqual:
                    stringFilterType = StringFilterType.NotEquals;
                    return true;

                default:
                    stringFilterType = StringFilterType.Equals;
                    return false;
            }
        }

        private bool TryConvertMethodCallToStringFilterType(string methodName, out StringFilterType stringFilterType)
        {
            switch (methodName)
            {
                case "StartsWith":
                    stringFilterType = StringFilterType.StartsWith;
                    return true;

                case "Contains":
                    stringFilterType = StringFilterType.Contains;
                    return true;

                default:
                    stringFilterType = StringFilterType.Equals;
                    return false;
            }
        }
    }
}

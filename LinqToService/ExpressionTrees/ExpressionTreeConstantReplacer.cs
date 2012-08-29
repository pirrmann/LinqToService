using System;
using System.Linq;
using System.Linq.Expressions;

namespace LinqToService.ExpressionTrees
{
    internal class ExpressionTreeConstantReplacer<TReplacement>
        : ExpressionVisitor
    {
        private Type originalType;
        private TReplacement replacementConstant;

        internal ExpressionTreeConstantReplacer(
            Type originalType, TReplacement replacementConstant)
        {
            this.originalType = originalType;
            this.replacementConstant = replacementConstant;
        }

        protected override Expression VisitConstant(
            ConstantExpression c)
        {
            if (c.Type == this.originalType)
                return Expression.Constant(this.replacementConstant);
            else
                return c;
        }
    }

    internal class ExpressionTreeConstantReplacer
    {
        internal static Expression CopyAndReplace<TReplacement>(
            Expression expression,
            Type originalType,
            TReplacement replacementConstant)
        {
            var modifier =
                new ExpressionTreeConstantReplacer<TReplacement>(
                    originalType,
                    replacementConstant);
            return modifier.Visit(expression);
        }
    }
}
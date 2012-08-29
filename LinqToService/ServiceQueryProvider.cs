using System;
using System.Linq;
using System.Linq.Expressions;

namespace LinqToService
{
    public class ServiceQueryProvider : IQueryProvider
    {
        public IQueryable CreateQuery(Expression expression)
        {
            Type elementType = TypeSystem.GetElementType(expression.Type);
            try
            {
                return (IQueryable)Activator.CreateInstance(typeof(QueryableServiceData<>).MakeGenericType(elementType), new object[] { this, expression });
            }
            catch (System.Reflection.TargetInvocationException tie)
            {
                throw tie.InnerException;
            }
        }

        // Queryable's collection-returning standard query operators call this method.
        public IQueryable<TResult> CreateQuery<TResult>(Expression expression)
        {
            return new QueryableServiceData<TResult>(this, expression);
        }

        public object Execute(Expression expression)
        {
            return ServiceQueryContext.Execute(expression, false);
        }

        // Queryable's "single value" standard query operators call this method.
        // It is also called from QueryableServiceData.GetEnumerator().
        public TResult Execute<TResult>(Expression expression)
        {
            bool IsEnumerable = (typeof(TResult).Name == "IEnumerable`1");

            return (TResult)ServiceQueryContext.Execute(expression, IsEnumerable);
        }
    }
}

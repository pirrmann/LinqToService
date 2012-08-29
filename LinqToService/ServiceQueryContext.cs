using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using LinqToService.ExpressionTrees;
using PeopleFinder;
using PeopleFinder.Criteria;

namespace LinqToService
{
    class ServiceQueryContext
    {
        private ServiceQueryContext() { }

        ////Step 0
        //internal static object Execute(Expression expression,
        //                                bool isEnumerable)
        //{
        //    // First, we call the web service and get an array of people
        //    IEnumerable<Person> people;

        //    using (DummyService.PeopleFinderClient service =
        //        new DummyService.PeopleFinderClient())
        //    {
        //        people = service.FindPeople(new DummyService.SearchCriteria());
        //    }

        //    // We use Ling to Objects to get an IQueryable instance of people
        //    var queryablePeople = people.AsQueryable();

        //    // We transform the expression tree
        //    Expression finalExpressionTree =
        //        ExpressionTreeConstantReplacer
        //            .CopyAndReplace(
        //                expression,
        //                typeof(QueryableDummyData<Person>),
        //                queryablePeople);

        //    // Finally, based on the new tree, we either create a query or
        //    // execute with the Linq to Objects provider
        //    IQueryProvider provider = queryablePeople.Provider;
        //    if (isEnumerable)
        //        return provider.CreateQuery(finalExpressionTree);
        //    else
        //        return provider.Execute(finalExpressionTree);
        //}

        internal static object Execute(Expression expression, bool isEnumerable)
        {
            SearchCriteriaBuilder searchCriteriaBuilder = new SearchCriteriaBuilder();
            Expression filteredExpressionTree = searchCriteriaBuilder.BuildCriteria(expression);
            SearchCriteria criteria = searchCriteriaBuilder.SearchCriteria;

            IPeopleFinder service = PeopleFinderLocator.GetInstance();

            var queryablePeople = service.FindPeople(criteria).AsQueryable();

            Expression finalExpressionTree =
                ExpressionTreeConstantReplacer
                    .CopyAndReplace(
                        filteredExpressionTree,
                        typeof(QueryableServiceData<Person>),
                        queryablePeople);

            string visu1 = TreeVisualizer.BuildVisualization(expression);
            string visu2 = TreeVisualizer.BuildVisualization(finalExpressionTree);

            // Finally, based on the new tree, we either create a query or execute
            // with the Linq to Objects provider
            if (isEnumerable)
                return queryablePeople.Provider.CreateQuery(finalExpressionTree);
            else
                return queryablePeople.Provider.Execute(finalExpressionTree);
        }
    }
}

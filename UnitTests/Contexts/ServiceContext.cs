using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using PeopleFinder;
using PeopleFinder.Criteria;

namespace Tests.Context
{
    public class ServiceContext
    {
        // Data context
        public IEnumerable<Person> AllPeople { get; set; }

        // Input
        public List<string> Query { get; set; }
        public List<string> ServiceLambdas { get; set; }

        // Service predicate, built upon the ServiceLambdas
        public Expression<Func<Person, bool>>
                            ServicePredicate {get; set;}

        // Output
        public List<Person> Results { get; set; }
        public SearchCriteria PassedCriteria { get; set; }
    }
}
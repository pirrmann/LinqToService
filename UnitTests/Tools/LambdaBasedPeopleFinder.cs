using System;
using System.Collections.Generic;
using System.Linq;
using PeopleFinder;
using PeopleFinder.Criteria;

namespace Tests.Tools
{
    public class LambdaBasedPeopleFinder : IPeopleFinder
    {
        private readonly IEnumerable<Person> people;
        private readonly Func<Person, bool> predicate;

        public LambdaBasedPeopleFinder(IEnumerable<Person> people, Func<Person, bool> predicate)
        {
            this.people = people;
            this.predicate = predicate;
        }

        public IEnumerable<Person> FindPeople(SearchCriteria criteria)
        {
            this.PassedCriteria = criteria;
            return people.Where(p => this.predicate(p));
        }

        public SearchCriteria PassedCriteria{ get; private set; }
    }
}
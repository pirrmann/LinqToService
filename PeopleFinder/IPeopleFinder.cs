using System;
using System.Collections.Generic;
using PeopleFinder.Criteria;

namespace PeopleFinder
{
    public interface IPeopleFinder
    {
        IEnumerable<Person> FindPeople(SearchCriteria criteria);
    }
}

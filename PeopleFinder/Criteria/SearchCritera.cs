using System;
using System.Collections.Generic;

namespace PeopleFinder.Criteria
{
    public class StringCriterion : Criterion<StringFilterType, string>
    {
    }

    public class IntCriterion : Criterion<NumericFilterType, int>
    {
    }

    public class SearchCriteria
    {
        public StringCriterion FirstName { get; set; }
        public StringCriterion LastName { get; set; }
        public IntCriterion Age { get; set; }
    }
}
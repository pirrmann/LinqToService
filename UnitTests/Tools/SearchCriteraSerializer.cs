using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PeopleFinder.Criteria;

namespace Tests.Tools
{
    public static class SearchCriteraSerializer
    {
        public static string Serialize(SearchCriteria criteria)
        {
            List<string> properties = new List<string>();
            if (criteria.FirstName != null) properties.Add(Serialize("FirstName", criteria.FirstName));
            if (criteria.LastName != null) properties.Add(Serialize("LastName", criteria.LastName));
            if (criteria.Age != null) properties.Add(Serialize("Age", criteria.Age));
            return String.Join(", ", properties);
        }

        public static string Serialize<TFilterType, TValue>(string name, Criterion<TFilterType, TValue> criterion)
            where TFilterType : struct
        {
            return string.Format(
                "{0} {1} {2}",
                name,
                criterion.FilterType.ToString(),
                criterion.Value.ToString());
        }
    }
}

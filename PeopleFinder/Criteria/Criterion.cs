using System;
using System.Collections.Generic;

namespace PeopleFinder.Criteria
{
    public abstract class Criterion<TFilterType, TValue>
        where TFilterType : struct
    {
        public TFilterType FilterType { get; set; }
        public TValue Value { get; set; }

        public override int GetHashCode()
        {
            return Value == null
                ? FilterType.GetHashCode()
                : FilterType.GetHashCode() ^ Value.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            Criterion<TFilterType, TValue> casted = obj as Criterion<TFilterType, TValue>;

            if (casted == null)
                return false;
            else
            {
                return this.FilterType.Equals(casted.FilterType)
                    && ((this.Value == null && casted.Value == null) || this.Value.Equals(casted.Value));
            }
        }
    }
}

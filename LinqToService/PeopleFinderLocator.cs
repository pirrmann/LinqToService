using System;
using PeopleFinder;

namespace LinqToService
{
    public static class PeopleFinderLocator
    {
        public static IPeopleFinder Instance;

        public static IPeopleFinder GetInstance()
        {
            return Instance;
        }
    }
}

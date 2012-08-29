//using System;
//using System.Linq;
//using System.Text;
//using System.Collections.Generic;
//using Microsoft.VisualStudio.TestTools.UnitTesting;
//using PeopleFinder;
//using PeopleFinder.Criteria;
//using LinqToService;

//namespace Tests
//{
//    [TestClass]
//    public class UnitTests
//    {
//        [TestMethod]
//        public void GivenAProviderWhenIGetAnEnumeratorThenICanMoveNext()
//        {
//            LambdaBasedPeopleFinder peopleFinder = new LambdaBasedPeopleFinder(p => true);
//            PeopleFinderLocator.Instance = peopleFinder;

//            var queryablePeople =
//                new QueryableServiceData<Person>();

//            bool exists = false;
//            using (IEnumerator<Person> enumerator =
//                queryablePeople.GetEnumerator())
//            {
//                exists = enumerator.MoveNext();
//            }

//            Assert.IsTrue(exists);
//        }

//        [TestMethod]
//        public void GivenAProviderWhenIFilterOnAgeGreaterThan36ThenItFilters()
//        {
//            LambdaBasedPeopleFinder peopleFinder = new LambdaBasedPeopleFinder(p => p.Age >= 36);
//            PeopleFinderLocator.Instance = peopleFinder;

//            QueryableServiceData<Person> people = new QueryableServiceData<Person>();

//            var query = people
//                .Where(p => p.Age >= 36);

//            var results = query.ToList();

//            Assert.IsTrue(results.All(p => p.Age >= 36));
//            Assert.AreEqual(
//                new IntCriterion()
//                    {
//                        FilterType = NumericFilterType.IsGreaterThan,
//                        Value = 36
//                    },
//                    peopleFinder.PassedCriteria.Age);
//        }

//        [TestMethod]
//        public void GivenAProviderWhenIFilterOnAgeGreaterThan36ThenTheProviderDoesntFilter()
//        {
//            LambdaBasedPeopleFinder peopleFinder = new LambdaBasedPeopleFinder(p => true);
//            PeopleFinderLocator.Instance = peopleFinder;

//            QueryableServiceData<Person> people = new QueryableServiceData<Person>();

//            var query = people
//                .Where(p => p.Age >= 36);

//            var results = query.ToList();

//            Assert.IsFalse(results.All(p => p.Age >= 36));
//            Assert.AreEqual(
//                new IntCriterion()
//                {
//                    FilterType = NumericFilterType.IsGreaterThan,
//                    Value = 36
//                },
//                peopleFinder.PassedCriteria.Age);
//        }

//        [TestMethod]
//        public void GivenAProviderWhenIFilterOnAgeStrictlyGreaterThan36ThenItFilters()
//        {
//            LambdaBasedPeopleFinder peopleFinder = new LambdaBasedPeopleFinder(p => p.Age > 36);
//            PeopleFinderLocator.Instance = peopleFinder;

//            QueryableServiceData<Person> people = new QueryableServiceData<Person>();

//            var query = people
//                .Where(p => p.Age > 36);

//            var results = query.ToList();

//            Assert.AreEqual(1, results.Count());
//            Assert.IsTrue(results.All(p => p.Age > 36));
//            Assert.AreEqual(
//                new IntCriterion()
//                {
//                    FilterType = NumericFilterType.IsStrictlyGreaterThan,
//                    Value = 36
//                },
//                    peopleFinder.PassedCriteria.Age);
//        }

//        [TestMethod]
//        public void GivenAProviderWhenIFilterOnAgeLessThan36ThenItFilters()
//        {
//            LambdaBasedPeopleFinder peopleFinder = new LambdaBasedPeopleFinder(p => p.Age <= 36);
//            PeopleFinderLocator.Instance = peopleFinder;

//            QueryableServiceData<Person> people = new QueryableServiceData<Person>();

//            var query = people
//                .Where(p => p.Age <= 36);

//            var results = query.ToList();

//            Assert.AreEqual(5, results.Count());
//            Assert.IsTrue(results.All(p => p.Age <= 36));
//            Assert.AreEqual(
//                new IntCriterion()
//                {
//                    FilterType = NumericFilterType.IsLessThan,
//                    Value = 36
//                },
//                peopleFinder.PassedCriteria.Age);
//        }

//        [TestMethod]
//        public void GivenAProviderWhenIFilterOnAgeStrictlyLessThan36ThenItFilters()
//        {
//            LambdaBasedPeopleFinder peopleFinder = new LambdaBasedPeopleFinder(p => p.Age < 36);
//            PeopleFinderLocator.Instance = peopleFinder;

//            QueryableServiceData<Person> people = new QueryableServiceData<Person>();

//            var query = people
//                .Where(p => p.Age < 36);

//            var results = query.ToList();

//            Assert.AreEqual(3, results.Count());
//            Assert.IsTrue(results.All(p => p.Age < 36));
//            Assert.AreEqual(
//                new IntCriterion()
//                {
//                    FilterType = NumericFilterType.IsStrictlyLessThan,
//                    Value = 36
//                },
//                peopleFinder.PassedCriteria.Age);
//        }

//        [TestMethod]
//        public void GivenAProviderWhenIFilterOnAgeEquals31ThenItFilters()
//        {
//            LambdaBasedPeopleFinder peopleFinder = new LambdaBasedPeopleFinder(p => p.Age == 31);
//            PeopleFinderLocator.Instance = peopleFinder;

//            QueryableServiceData<Person> people = new QueryableServiceData<Person>();

//            var query = people
//                .Where(p => p.Age == 31);

//            var results = query.ToList();

//            Assert.AreEqual(2, results.Count());
//            Assert.IsTrue(results.All(p => p.Age == 31));
//            Assert.AreEqual(
//                new IntCriterion()
//                {
//                    FilterType = NumericFilterType.Equals,
//                    Value = 31
//                },
//                peopleFinder.PassedCriteria.Age);
//        }

//        [TestMethod]
//        public void GivenAProviderWhenIFilterOnAgeNotEquals31ThenItFilters()
//        {
//            LambdaBasedPeopleFinder peopleFinder = new LambdaBasedPeopleFinder(p => p.Age != 31);
//            PeopleFinderLocator.Instance = peopleFinder;

//            QueryableServiceData<Person> people = new QueryableServiceData<Person>();

//            var query = people
//                .Where(p => p.Age != 31);

//            var results = query.ToList();

//            Assert.AreEqual(4, results.Count());
//            Assert.IsTrue(results.All(p => p.Age != 31));
//            Assert.AreEqual(
//                new IntCriterion()
//                {
//                    FilterType = NumericFilterType.NotEquals,
//                    Value = 31
//                },
//                peopleFinder.PassedCriteria.Age);
//        }

//        //[TestMethod]
//        //public void NewStupidTest()
//        //{
//        //    LambdaBasedPeopleFinder peopleFinder = new LambdaBasedPeopleFinder(p => true);
//        //    PeopleFinderLocator.Instance = peopleFinder;

//        //    QueryableServiceData<Person> people = new QueryableServiceData<Person>();

//        //    var query = people
//        //        .OrderBy(p => p.LastName)
//        //        .Take(3)
//        //        .Where(p => p.Age <= 31);

//        //    var results = query.ToList();

//        //    Assert.AreEqual(2, results.Count());
//        //    Assert.IsNull(peopleFinder.PassedCriteria.Age);
//        //}
//    }
//}

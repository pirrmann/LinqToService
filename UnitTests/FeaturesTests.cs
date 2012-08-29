using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using LinqToService;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PeopleFinder;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;
using Tests.Context;
using Tests.Tools;

namespace Tests
{
    [Binding]
    public class FeaturesTests
    {
        private readonly ServiceContext context;
        private readonly CommonContext commonContext;

        public FeaturesTests(ServiceContext context, CommonContext commonContext)
        {
            this.context = context;
            this.commonContext = commonContext;
        }

        [Given(@"The people are")]
        public void GivenThePeopleAre(Table people)
        {
            this.context.AllPeople = people.CreateSet<Person>();
        }

        [Given(@"I have written a query against the provider")]
        public void GivenIHaveWrittenAQuery()
        {
            this.context.Query = new List<string>();
            this.context.ServiceLambdas = new List<string>();
        }

        [Given(@"I have added a (.*) where clause")]
        public void GivenIHaveAddedAWhereClause(string predicate)
        {
            this.context.Query.Add(predicate);
        }

        [Given(@"The people finder service filters on (.*)")]
        public void GivenThePeopleFinderServiceFiltersOn(string servicePredicate)
        {
            this.context.ServiceLambdas.Add(servicePredicate);
        }

        [When(@"I execute the query")]
        public void WhenIExecuteTheQuery()
        {
            try
            {
                this.context.ServicePredicate = (Expression<Func<Person, bool>>)this.context.ServiceLambdas
                    .Select(s => LambdaParser.ParseWhere(s))
                    .Aggregate(
                    (Expression)null, (a, e) => a == null ? e : (Expression)Expression.AndAlso(a, e));

                LambdaBasedPeopleFinder peopleFinder = new LambdaBasedPeopleFinder(
                    this.context.AllPeople,
                    this.context.ServicePredicate.Compile());
                PeopleFinderLocator.Instance = peopleFinder;
                QueryableServiceData<Person> people = new QueryableServiceData<Person>();

                IQueryable<Person> query = people;
                foreach(string where in this.context.Query)
                {
                    query = query.Where(LambdaParser.ParseWhere(where));
                }

                this.context.Results = query.ToList();
                this.context.PassedCriteria = peopleFinder.PassedCriteria;
            }
            catch (Exception ex)
            {
                this.commonContext.Exception = ex;
            }
        }

        [Then(@"The service parameter should be (.*)")]
        public void ThenTheServiceParameter(string serviceParameter)
        {
            Assert.AreEqual(serviceParameter, SearchCriteraSerializer.Serialize(this.context.PassedCriteria));
        }

        [Then(@"The result count should be (.*)")]
        public void ThenTheResultCount(int count)
        {
            Assert.AreEqual(count, this.context.Results.Count);
        }

        [Then(@"The result should validate the servicePredicate")]
        public void ThenTheResultShouldValidateTheServicePredicate()
        {
            foreach(string lambda in this.context.ServiceLambdas)
                foreach(var res in this.context.Results)
                    Assert.IsTrue(LambdaParser.ParseWhere(lambda).Compile().Invoke(res));
        }

        [Then(@"An error should occur")]
        public void ThenAnErrorShouldOccur()
        {
            Assert.IsNotNull(this.commonContext.Exception);
        }
    }
}
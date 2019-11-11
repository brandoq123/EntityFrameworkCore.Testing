﻿using System.Linq;
using EntityFrameworkCore.Testing.Common.Tests;
using NUnit.Framework;

namespace EntityFrameworkCore.Testing.Moq.Tests
{
    [TestFixture]
    public class ByTypeDbQueryTests : DbQueryTestsBase<TestQuery>
    {
        protected override IQueryable<TestQuery> Queryable => MockedDbContext.Query<TestQuery>();
    }
}
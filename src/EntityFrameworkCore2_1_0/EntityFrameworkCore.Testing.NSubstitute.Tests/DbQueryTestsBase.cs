﻿using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using EntityFrameworkCore.Testing.Common.Tests;
using EntityFrameworkCore.Testing.NSubstitute.Extensions;
using EntityFrameworkCore.Testing.NSubstitute.Helpers;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace EntityFrameworkCore.Testing.NSubstitute.Tests
{
    [TestFixture]
    public abstract class DbQueryTestsBase<T> : Common.Tests.DbQueryTestsBase<T> where T : TestEntityBase
    {
        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
            var dbContextToMock = new TestDbContext(new DbContextOptionsBuilder<TestDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);
            MockedDbContext = Create.SubstituteFor(dbContextToMock);
        }

        protected TestDbContext MockedDbContext;

        protected override void AddFromSqlResult(IQueryable<T> mockedQueryable, IEnumerable<T> expectedResult)
        {
            mockedQueryable.AddFromSqlResult(expectedResult);
        }

        protected override void AddFromSqlResult(IQueryable<T> mockedQueryable, string sql, IEnumerable<T> expectedResult)
        {
            mockedQueryable.AddFromSqlResult(sql, expectedResult);
        }

        protected override void AddFromSqlResult(IQueryable<T> mockedQueryable, string sql, List<SqlParameter> parameters, IEnumerable<T> expectedResult)
        {
            mockedQueryable.AddFromSqlResult(sql, parameters, expectedResult);
        }

        protected override void AddToReadOnlySource(DbQuery<T> mockedDbQuery, T item)
        {
            mockedDbQuery.AddToReadOnlySource(item);
        }

        protected override void AddRangeToReadOnlySource(DbQuery<T> mockedDbQuery, IEnumerable<T> enumerable)
        {
            mockedDbQuery.AddRangeToReadOnlySource(enumerable);
        }

        protected override void ClearReadOnlySource(DbQuery<T> mockedDbQuery)
        {
            mockedDbQuery.ClearReadOnlySource();
        }
    }
}
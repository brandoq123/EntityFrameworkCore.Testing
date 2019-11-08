﻿using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace EntityFrameworkCore.Testing.Common.Tests
{
    [TestFixture]
    public abstract class ExecuteSqlCommandTestsBase<TDbContext> : TestBase
        where TDbContext : DbContext
    {
        protected TDbContext MockedDbContext;

        public abstract void AddExecuteSqlCommandResult(TDbContext mockedDbContext, int expectedResult);
        public abstract void AddExecuteSqlCommandResult(TDbContext mockedDbContext, string sql, int expectedResult);
        public abstract void AddExecuteSqlCommandResult(TDbContext mockedDbContext, string sql, List<SqlParameter> parameters, int expectedResult);

        [Test]
        public void ExecuteSqlCommand_AnySql_ReturnsExpectedResult()
        {
            var expectedResult = 1;
            AddExecuteSqlCommandResult(MockedDbContext, expectedResult);

            var actualResult1 = MockedDbContext.Database.ExecuteSqlCommand("sp_NoParams");
            var actualResult2 = MockedDbContext.Database.ExecuteSqlCommand("sp_NoParams");

            Assert.Multiple(() =>
            {
                Assert.That(actualResult1, Is.EqualTo(expectedResult));
                Assert.That(actualResult2, Is.EqualTo(actualResult1));
            });
        }

        [Test]
        public void ExecuteSqlCommand_SpecifiedSql_ReturnsExpectedResult()
        {
            var sql = "sp_NoParams";
            var expectedResult = 1;
            AddExecuteSqlCommandResult(MockedDbContext, sql, expectedResult);

            var actualResult1 = MockedDbContext.Database.ExecuteSqlCommand("sp_NoParams");
            var actualResult2 = MockedDbContext.Database.ExecuteSqlCommand("sp_NoParams");

            Assert.Multiple(() =>
            {
                Assert.That(actualResult1, Is.EqualTo(expectedResult));
                Assert.That(actualResult2, Is.EqualTo(actualResult1));
            });
        }

        [Test]
        public void ExecuteSqlCommand_SpecifiedSqlThatDoesNotMatchSetUp_ThrowsException()
        {
            var sql = "asdf";
            var expectedResult = 1;
            AddExecuteSqlCommandResult(MockedDbContext, sql, expectedResult);

            Assert.Throws<NullReferenceException>(() =>
            {
                var actualResult = MockedDbContext.Database.ExecuteSqlCommand("sp_NoParams");
            });
        }

        [Test]
        public void ExecuteSqlCommand_SpecifiedSqlWithParameters_ReturnsExpectedResult()
        {
            var sql = "sp_WithParams";
            var parameters = new List<SqlParameter> {new SqlParameter("@SomeParameter2", "Value2")};
            var expectedResult = 1;
            AddExecuteSqlCommandResult(MockedDbContext, sql, parameters, expectedResult);

            var actualResult1 = MockedDbContext.Database.ExecuteSqlCommand("[dbo].[sp_WithParams] @SomeParameter1 @SomeParameter2", parameters);
            var actualResult2 = MockedDbContext.Database.ExecuteSqlCommand("[dbo].[sp_WithParams] @SomeParameter1 @SomeParameter2", parameters);

            Assert.Multiple(() =>
            {
                Assert.That(actualResult1, Is.EqualTo(expectedResult));
                Assert.That(actualResult2, Is.EqualTo(actualResult1));
            });
        }

        [Test]
        public void ExecuteSqlCommand_WithNoMatchesAdded_ThrowsException()
        {
            Assert.Throws<InvalidOperationException>(() =>
            {
                var actualResult = MockedDbContext.Database.ExecuteSqlCommand("sp_NoParams");
            });
        }

        [Test]
        public async Task ExecuteSqlCommandAsync_AnySql_ReturnsExpectedResult()
        {
            var sql = "";
            var expectedResult = 1;
            AddExecuteSqlCommandResult(MockedDbContext, sql, expectedResult);

            var actualResult1 = await MockedDbContext.Database.ExecuteSqlCommandAsync("sp_NoParams");
            var actualResult2 = await MockedDbContext.Database.ExecuteSqlCommandAsync("sp_NoParams");

            Assert.Multiple(() =>
            {
                Assert.That(actualResult1, Is.EqualTo(expectedResult));
                Assert.That(actualResult2, Is.EqualTo(actualResult1));
            });
        }

        [Test]
        public async Task ExecuteSqlCommandAsync_SpecifiedSql_ReturnsExpectedResult()
        {
            var sql = "sp_NoParams";
            var expectedResult = 1;
            AddExecuteSqlCommandResult(MockedDbContext, sql, expectedResult);

            var actualResult1 = await MockedDbContext.Database.ExecuteSqlCommandAsync("sp_NoParams");
            var actualResult2 = await MockedDbContext.Database.ExecuteSqlCommandAsync("sp_NoParams");

            Assert.Multiple(() =>
            {
                Assert.That(actualResult1, Is.EqualTo(expectedResult));
                Assert.That(actualResult2, Is.EqualTo(actualResult1));
            });
        }

        [Test]
        public void ExecuteSqlCommandAsync_SpecifiedSqlThatDoesNotMatchSetUp_ThrowsException()
        {
            var sql = "asdf";
            var expectedResult = 1;
            AddExecuteSqlCommandResult(MockedDbContext, sql, expectedResult);

            Assert.ThrowsAsync<NullReferenceException>(async () =>
            {
                var actualResult = await MockedDbContext.Database.ExecuteSqlCommandAsync("sp_NoParams");
            });
        }

        [Test]
        public async Task ExecuteSqlCommandAsync_SpecifiedSqlWithParameters_ReturnsExpectedResult()
        {
            var sql = "sp_WithParams";
            var parameters = new List<SqlParameter> {new SqlParameter("@SomeParameter2", "Value2")};
            var expectedResult = 1;
            AddExecuteSqlCommandResult(MockedDbContext, sql, parameters, expectedResult);

            var actualResult1 = await MockedDbContext.Database.ExecuteSqlCommandAsync("[dbo].[sp_WithParams] @SomeParameter2", parameters);
            var actualResult2 = await MockedDbContext.Database.ExecuteSqlCommandAsync("[dbo].[sp_WithParams] @SomeParameter2", parameters);

            Assert.Multiple(() =>
            {
                Assert.That(actualResult1, Is.EqualTo(expectedResult));
                Assert.That(actualResult2, Is.EqualTo(actualResult1));
            });
        }
    }
}
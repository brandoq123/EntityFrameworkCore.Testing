using System;
using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using EntityFrameworkCore.Testing.Common.Helpers;
using EntityFrameworkCore.Testing.Common.Tests;
using EntityFrameworkCore.Testing.Moq.Extensions;
using EntityFrameworkCore.Testing.Moq.Helpers;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;

namespace EntityFrameworkCore.Testing.Moq.PackageVerification.Tests
{
    public class Tests
    {
        public Fixture Fixture = new Fixture();

        [SetUp]
        public virtual void SetUp()
        {
            //LoggerHelper.LoggerFactory
        }

        [Test]
        public void SetAddAndPersist_Item_Persists()
        {
            var dbContextToMock = new TestDbContext(new DbContextOptionsBuilder<TestDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);
            var mockedDbContext = Create.MockedDbContextFor(dbContextToMock);

            var testEntity = Fixture.Create<TestEntity>();

            mockedDbContext.Set<TestEntity>().Add(testEntity);
            mockedDbContext.SaveChanges();

            Assert.Multiple(() =>
            {
                Assert.AreNotEqual(default(Guid), testEntity.Guid);
                Assert.DoesNotThrow(() => mockedDbContext.Set<TestEntity>().Single());
                Assert.AreEqual(testEntity, mockedDbContext.Find<TestEntity>(testEntity.Guid));
            });
        }

        [Test]
        public void FromSqlRaw_AnyStoredProcedureWithNoParameters_ReturnsExpectedResult()
        {
            var dbContextToMock = new TestDbContext(new DbContextOptionsBuilder<TestDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);
            var mockedDbContext = Create.MockedDbContextFor(dbContextToMock);

            var expectedResult = Fixture.CreateMany<TestEntity>().ToList();

            mockedDbContext.Set<TestEntity>().AddFromSqlRawResult(expectedResult);

            var actualResult = mockedDbContext.Set<TestEntity>().FromSqlRaw("sp_NoParams").ToList();

            Assert.Multiple(() =>
            {
                Assert.IsNotNull(actualResult);
                Assert.IsTrue(actualResult.Any());
                CollectionAssert.AreEquivalent(expectedResult, actualResult);
            });
        }

        [Test]
        public void FromSqlRaw_SpecifiedStoredProcedureAndParameters_ReturnsExpectedResult()
        {
            var dbContextToMock = new TestDbContext(new DbContextOptionsBuilder<TestDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);
            var mockedDbContext = Create.MockedDbContextFor(dbContextToMock);

            var sqlParameters = new List<SqlParameter> {new SqlParameter("@SomeParameter2", "Value2")};
            var expectedResult = Fixture.CreateMany<TestEntity>().ToList();

            mockedDbContext.Set<TestEntity>().AddFromSqlRawResult("sp_Specified", sqlParameters, expectedResult);

            var actualResult = mockedDbContext.Set<TestEntity>().FromSqlRaw("[dbo].[sp_Specified] @SomeParameter1 @SomeParameter2", new SqlParameter("@someparameter2", "Value2")).ToList();

            Assert.Multiple(() =>
            {
                Assert.IsNotNull(actualResult);
                Assert.IsTrue(actualResult.Any());
                CollectionAssert.AreEquivalent(expectedResult, actualResult);
            });
        }

        [Test]
        public void QueryAddRangeToReadOnlySource_Enumeration_AddsToQuerySource()
        {
            var dbContextToMock = new TestDbContext(new DbContextOptionsBuilder<TestDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);
            var mockedDbContext = Create.MockedDbContextFor(dbContextToMock);

            var expectedResult = Fixture.CreateMany<TestQuery>().ToList();

            mockedDbContext.Query<TestQuery>().AddRangeToReadOnlySource(expectedResult);

            Assert.Multiple(() =>
            {
                CollectionAssert.AreEquivalent(expectedResult, mockedDbContext.Query<TestQuery>().ToList());
                CollectionAssert.AreEquivalent(mockedDbContext.Query<TestQuery>().ToList(), mockedDbContext.TestView.ToList());
            });
        }

        [Test]
        public void ExecuteSqlCommand_SpecifiedStoredProcedure_ReturnsExpectedResult()
        {
            var dbContextToMock = new TestDbContext(new DbContextOptionsBuilder<TestDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);
            var mockedDbContext = Create.MockedDbContextFor(dbContextToMock);

            var commandText = "sp_NoParams";
            var expectedResult = 1;

            mockedDbContext.AddExecuteSqlCommandResult(commandText, new List<SqlParameter>(), expectedResult);

            var result = mockedDbContext.Database.ExecuteSqlCommand("sp_NoParams");

            Assert.AreEqual(expectedResult, result);
        }

        [Test]
        public void ExecuteSqlCommand_SpecifiedStoredProcedureAndSqlParameters_ReturnsExpectedResult()
        {
            var dbContextToMock = new TestDbContext(new DbContextOptionsBuilder<TestDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);
            var mockedDbContext = Create.MockedDbContextFor(dbContextToMock);

            var commandText = "sp_WithParams";
            var sqlParameters = new List<SqlParameter> {new SqlParameter("@SomeParameter2", "Value2")};
            var expectedResult = 1;

            mockedDbContext.AddExecuteSqlCommandResult(commandText, sqlParameters, expectedResult);

            var result = mockedDbContext.Database.ExecuteSqlCommand("[dbo].[sp_WithParams] @SomeParameter2", sqlParameters);

            Assert.AreEqual(expectedResult, result);
        }

        [Test]
        public void AddRangeThenSaveChanges_CanAssertInvocationCount()
        {
            var dbContextToMock = new TestDbContext(new DbContextOptionsBuilder<TestDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);
            var mockedDbContext = Create.MockedDbContextFor(dbContextToMock);

            mockedDbContext.Set<TestEntity>().AddRange(Fixture.CreateMany<TestEntity>().ToList());
            mockedDbContext.SaveChanges();

            Assert.Multiple(() =>
            {
                var dbContextMock = Mock.Get(mockedDbContext);
                dbContextMock.Verify(m => m.SaveChanges(), Times.Once);

                //The db set is a mock, so we need to ensure we invoke the verify on the db set mock
                var byTypeDbSetMock = Mock.Get(mockedDbContext.Set<TestEntity>());
                byTypeDbSetMock.Verify(m => m.AddRange(It.IsAny<IEnumerable<TestEntity>>()), Times.Once);

                //This is the same mock instance as above, just accessed a different way
                var byPropertyDbSetMock = Mock.Get(mockedDbContext.TestEntities);

                Assert.That(byPropertyDbSetMock, Is.SameAs(byTypeDbSetMock));

                byPropertyDbSetMock.Verify(m => m.AddRange(It.IsAny<IEnumerable<TestEntity>>()), Times.Once);
            });
        }
    }
}
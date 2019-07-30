﻿using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using NHSD.GPITF.BuyingCatalog.Datastore.Database.Interfaces;
using NHSD.GPITF.BuyingCatalog.Interfaces;
using NHSD.GPITF.BuyingCatalog.Models;
using NUnit.Framework;
using System.Reflection;

namespace NHSD.GPITF.BuyingCatalog.Datastore.Database.Tests
{
  [TestFixture]
  public sealed class ReviewsDatastoreBase_Tests
  {
    private Mock<IDbConnectionFactory> _dbConnectionFactory;
    private Mock<ILogger<DummyReviewsDatastoreBase>> _logger;
    private Mock<ISyncPolicyFactory> _policy;

    [SetUp]
    public void SetUp()
    {
      _dbConnectionFactory = new Mock<IDbConnectionFactory>();
      _logger = new Mock<ILogger<DummyReviewsDatastoreBase>>();
      _policy = new Mock<ISyncPolicyFactory>();
    }

    [Test]
    public void Constructor_Completes()
    {
      Assert.DoesNotThrow(() => new DummyReviewsDatastoreBase(_dbConnectionFactory.Object, _logger.Object, _policy.Object));
    }

    [Test]
    public void Class_Implements_Interface()
    {
      var obj = new DummyReviewsDatastoreBase(_dbConnectionFactory.Object, _logger.Object, _policy.Object);

      var implInt = obj as IReviewsDatastore<ReviewsBase>;

      implInt.Should().NotBeNull();
    }

    [Test]
    public void GetSqlCurrent_ContainsAllPublicProperties()
    {
      const string TableName = "Reviews";

      var props = typeof(ReviewsBase).GetProperties(BindingFlags.Public | BindingFlags.Instance);
      var datastore = new DummyReviewsDatastoreBase(_dbConnectionFactory.Object, _logger.Object, _policy.Object);

      var sqlCurrent = datastore.GetSqlCurrent(TableName);


      foreach (var prop in props)
      {
        sqlCurrent.Should().Contain($"{TableName}.{prop.Name}");
      }
    }
  }
}

using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using NHSD.GPITF.BuyingCatalog.Search.Porcelain;
using NHSD.GPITF.BuyingCatalog.Interfaces;
using NHSD.GPITF.BuyingCatalog.Interfaces.Porcelain;
using NUnit.Framework;
using Polly;
using System.Collections.Generic;
using NHSD.GPITF.BuyingCatalog.Models.Porcelain;
using System;
using NHSD.GPITF.BuyingCatalog.Tests;

namespace NHSD.GPITF.BuyingCatalog.Search.Tests
{
  [TestFixture]
  public sealed class SearchDatastore_Tests
  {
    private Mock<ILogger<SearchDatastore>> _logger;
    private Mock<ISyncPolicyFactory> _policyFact;
    private Mock<ISyncPolicy> _policy;
    private Mock<IFrameworksDatastore> _frameworkDatastore;
    private Mock<ISolutionsDatastore> _solutionDatastore;
    private Mock<ICapabilitiesDatastore> _capabilityDatastore;
    private Mock<ICapabilitiesImplementedDatastore> _claimedCapabilityDatastore;
    private Mock<ISolutionsExDatastore> _solutionExDatastore;

    [SetUp]
    public void SetUp()
    {
      _logger = new Mock<ILogger<SearchDatastore>>();
      _policyFact = new Mock<ISyncPolicyFactory>();
      _policy = new Mock<ISyncPolicy>();
      _frameworkDatastore = new Mock<IFrameworksDatastore>();
      _solutionDatastore = new Mock<ISolutionsDatastore>();
      _capabilityDatastore = new Mock<ICapabilitiesDatastore>();
      _claimedCapabilityDatastore = new Mock<ICapabilitiesImplementedDatastore>();
      _solutionExDatastore = new Mock<ISolutionsExDatastore>();

      _policyFact.Setup(x => x.Build(_logger.Object)).Returns(_policy.Object);
    }

    [Test]
    public void Constructor_Completes()
    {
      Assert.DoesNotThrow(() => Create());
    }

    private SearchDatastore Create()
    {
      return new SearchDatastore(
        _logger.Object,
        _policyFact.Object,
        _frameworkDatastore.Object,
        _solutionDatastore.Object,
        _capabilityDatastore.Object,
        _claimedCapabilityDatastore.Object,
        _solutionExDatastore.Object);
    }
  }
}

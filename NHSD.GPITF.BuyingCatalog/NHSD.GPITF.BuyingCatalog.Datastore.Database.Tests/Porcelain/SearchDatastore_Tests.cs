using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using NHSD.GPITF.BuyingCatalog.Datastore.Database.Interfaces;
using NHSD.GPITF.BuyingCatalog.Datastore.Database.Porcelain;
using NHSD.GPITF.BuyingCatalog.Interfaces;
using NHSD.GPITF.BuyingCatalog.Interfaces.Porcelain;
using NHSD.GPITF.BuyingCatalog.Models;
using NHSD.GPITF.BuyingCatalog.Models.Porcelain;
using NHSD.GPITF.BuyingCatalog.Tests;
using NUnit.Framework;
using Polly;
using System;
using System.Collections.Generic;

namespace NHSD.GPITF.BuyingCatalog.Datastore.Database.Tests.Porcelain
{
  [TestFixture]
  public sealed class SearchDatastore_Tests
  {
    private Mock<IDbConnectionFactory> _dbConnectionFactory;
    private Mock<ILogger<SearchDatastore>> _logger;
    private Mock<ISyncPolicyFactory> _policyFact;
    private Mock<ISyncPolicy> _policy;
    private Mock<IFrameworksDatastore> _frameworkDatastore;
    private Mock<ISolutionsDatastore> _solutionDatastore;
    private Mock<ICapabilitiesDatastore> _capabilityDatastore;
    private Mock<ICapabilitiesImplementedDatastore> _claimedCapabilityDatastore;
    private Mock<ISolutionsExDatastore> _solutionsExDatastore;

    [SetUp]
    public void SetUp()
    {
      _dbConnectionFactory = new Mock<IDbConnectionFactory>();
      _logger = new Mock<ILogger<SearchDatastore>>();
      _policyFact = new Mock<ISyncPolicyFactory>();
      _policy = new Mock<ISyncPolicy>();
      _frameworkDatastore = new Mock<IFrameworksDatastore>();
      _solutionDatastore = new Mock<ISolutionsDatastore>();
      _capabilityDatastore = new Mock<ICapabilitiesDatastore>();
      _claimedCapabilityDatastore = new Mock<ICapabilitiesImplementedDatastore>();
      _solutionsExDatastore = new Mock<ISolutionsExDatastore>();

      _policyFact.Setup(x => x.Build(_logger.Object)).Returns(_policy.Object);
    }

    [Test]
    public void Constructor_Completes()
    {
      Assert.DoesNotThrow(() => Create());
    }

    [TestCase("CapId_01")]
    public void ByCapabilities_SolutionHasCapability_ReturnsSolution(string capabilityId)
    {
      var framework = Creator.GetFramework();
      _frameworkDatastore.Setup(x => x.GetAll()).Returns(new[] { framework });

      var soln = Creator.GetSolution();
      _solutionDatastore.Setup(x => x.ByFramework(framework.Id)).Returns(new[] { soln });

      var capability = Creator.GetCapability(id: capabilityId);
      var claimedCapability = Creator.GetCapabilitiesImplemented(solnId: soln.Id, claimId: capability.Id);
      _claimedCapabilityDatastore.Setup(x => x.BySolution(soln.Id)).Returns(new[] { claimedCapability });

      var solnEx = Creator.GetSolutionEx(soln: soln);
      _solutionsExDatastore.Setup(x => x.BySolution(soln.Id)).Returns(solnEx);

      IEnumerable<SolutionEx> results = null;
      _policy.Setup(x => x.Execute(It.IsAny<Func<IEnumerable<SolutionEx>>>()))
        .Callback((Func<IEnumerable<SolutionEx>> action) => results = action())
        .Returns(results);

      var search = Create();

      search.ByCapabilities(new[] { capabilityId });

      var res = results.Should().ContainSingle();
      res.Which.Should().BeEquivalentTo(solnEx);
    }

    private SearchDatastore Create()
    {
      return new SearchDatastore(
      _dbConnectionFactory.Object,
      _logger.Object,
      _policyFact.Object,
      _frameworkDatastore.Object,
      _solutionDatastore.Object,
      _capabilityDatastore.Object,
      _claimedCapabilityDatastore.Object,
      _solutionsExDatastore.Object);
    }
  }
}

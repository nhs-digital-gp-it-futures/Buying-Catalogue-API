using FluentAssertions;
using Moq;
using NHSD.GPITF.BuyingCatalog.Interfaces;
using NHSD.GPITF.BuyingCatalog.Models;
using NHSD.GPITF.BuyingCatalog.Tests;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NHSD.GPITF.BuyingCatalog.Logic.Tests
{
  [TestFixture]
  public sealed class ClaimsFilterBase_Tests
  {
    private Mock<ISolutionsDatastore> _solutionDatastore;
    private Mock<ISolutionsFilter> _solutionsFilter;

    [SetUp]
    public void SetUp()
    {
      _solutionDatastore = new Mock<ISolutionsDatastore>();
      _solutionsFilter = new Mock<ISolutionsFilter>();
    }

    [Test]
    public void Constructor_Completes()
    {
      Assert.DoesNotThrow(() => Create());
    }

    [Test]
    public void Filter_None_Returns_Approved(
      [ValueSource(typeof(Creator), nameof(Creator.SolutionStatuses))]SolutionStatus status)
    {
      var filter = Create();
      var soln = Creator.GetSolution(status: status);
      _solutionDatastore.Setup(x => x.ById(soln.Id)).Returns(soln);
      var claim = Creator.GetClaimsBase(solnId: soln.Id);
      _solutionsFilter
        .Setup(x => x.Filter(It.Is<IEnumerable<Solutions>>(solns => solns.Contains(soln))))
        .Returns(soln.Status == SolutionStatus.Approved ? new[] { soln } : Enumerable.Empty<Solutions>());

      var res = filter.Filter(claim);

      res.Should().Be(soln.Status == SolutionStatus.Approved ? claim : null);
    }

    private DummyClaimsFilterBase Create()
    {
      return new DummyClaimsFilterBase(
        _solutionDatastore.Object,
        _solutionsFilter.Object);
    }
  }
}

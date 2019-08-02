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
    public void Filter_None_Returns_Filtered_By_SolutionFilter(
      [Values(true, false)]bool returnSoln)
    {
      var filter = Create();
      var soln = Creator.GetSolution();
      _solutionDatastore.Setup(x => x.ById(soln.Id)).Returns(soln);
      _solutionsFilter
        .Setup(x => x.Filter(It.Is<IEnumerable<Solutions>>(solns => solns.Contains(soln))))
        .Returns(returnSoln ? new[] { soln } : Enumerable.Empty<Solutions>());
      var claims = new[]
      {
        Creator.GetClaimsBase(solnId: soln.Id),
        Creator.GetClaimsBase(solnId: soln.Id),
        Creator.GetClaimsBase(solnId: soln.Id)
      };
      var expClaims = claims.Where(x => returnSoln);

      var res = filter.Filter(claims);

      res.Should().BeEquivalentTo(returnSoln ? claims : expClaims);
    }

    [Test]
    public void Filter_None_Calls_SolutionFilter(
      [Values(true, false)]bool returnSoln)
    {
      var filter = Create();
      var soln = Creator.GetSolution();
      _solutionDatastore.Setup(x => x.ById(soln.Id)).Returns(soln);
      _solutionsFilter
        .Setup(x => x.Filter(It.Is<IEnumerable<Solutions>>(solns => solns.Contains(soln))))
        .Returns(returnSoln ? new[] { soln } : Enumerable.Empty<Solutions>());
      var claims = new[]
      {
        Creator.GetClaimsBase(solnId: soln.Id),
        Creator.GetClaimsBase(solnId: soln.Id),
        Creator.GetClaimsBase(solnId: soln.Id)
      };

      // use ToList() to force LINQ to run
      filter.Filter(claims).ToList();

      _solutionsFilter.Verify(x => x.Filter(It.Is<IEnumerable<Solutions>>(solns => solns.Contains(soln))), Times.Exactly(claims.Count()));
    }

    private DummyClaimsFilterBase Create()
    {
      return new DummyClaimsFilterBase(
        _solutionDatastore.Object,
        _solutionsFilter.Object);
    }
  }
}

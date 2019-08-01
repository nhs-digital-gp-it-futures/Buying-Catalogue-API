using FluentAssertions;
using Microsoft.AspNetCore.Http;
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
    private Mock<IHttpContextAccessor> _context;
    private Mock<ISolutionsDatastore> _solutionDatastore;
    private Mock<ISolutionsFilter> _solutionsFilter;

    [SetUp]
    public void SetUp()
    {
      _context = new Mock<IHttpContextAccessor>();
      _solutionDatastore = new Mock<ISolutionsDatastore>();
      _solutionsFilter = new Mock<ISolutionsFilter>();
    }

    [Test]
    public void Constructor_Completes()
    {
      Assert.DoesNotThrow(() => Create());
    }

    [Test]
    public void Filter_Admin_Returns_All(
      [ValueSource(typeof(Creator), nameof(Creator.SolutionStatuses))]SolutionStatus status)
    {
      var filter = Create();
      var soln = Creator.GetSolution(status: status);
      _solutionDatastore.Setup(x => x.ById(soln.Id)).Returns(soln);
      var claim = Creator.GetClaimsBase(solnId: soln.Id);

      var res = filter.FilterForAdmin(claim);

      res.Should().Be(claim);
    }

    [Test]
    public void Filter_Buyer_Returns_NonFailedDraft(
      [ValueSource(typeof(Creator), nameof(Creator.SolutionStatuses))]SolutionStatus status)
    {
      var filter = Create();
      var soln = Creator.GetSolution(status: status);
      _solutionDatastore.Setup(x => x.ById(soln.Id)).Returns(soln);
      var claim = Creator.GetClaimsBase(solnId: soln.Id);
      _solutionsFilter
        .Setup(x => x.Filter(It.Is<IEnumerable<Solutions>>(solns => solns.Contains(soln))))
        .Returns(
          soln.Status != SolutionStatus.Draft &&
          soln.Status != SolutionStatus.Failed ?
            new[] { soln } : Enumerable.Empty<Solutions>());

      var res = filter.FilterForBuyer(claim);

      res.Should().Be(
          soln.Status != SolutionStatus.Draft &&
          soln.Status != SolutionStatus.Failed ?
            claim : null);
    }

    [Test]
    public void Filter_None_Returns_Approved(
      [ValueSource(typeof(Creator), nameof(Creator.SolutionStatuses))]SolutionStatus status)
    {
      var ctx = Creator.GetContext(role: "None");
      _context.Setup(c => c.HttpContext).Returns(ctx);
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

    [Test]
    public void Filter_SupplierOwn_ReturnsOwn()
    {
      var filter = Create();
      var orgId = Guid.NewGuid().ToString();
      var soln = Creator.GetSolution(orgId: orgId);
      var claim = Creator.GetClaimsBase(solnId: soln.Id);
      var ctx = Creator.GetContext(orgId: orgId);
      _context.Setup(c => c.HttpContext).Returns(ctx);
      _solutionDatastore.Setup(x => x.ById(soln.Id)).Returns(soln);

      var res = filter.FilterForSupplier(claim);

      res.Should().Be(claim);
    }

    [Test]
    public void Filter_SupplierOther_ReturnsNull()
    {
      var filter = Create();
      var orgId = Guid.NewGuid().ToString();
      var soln = Creator.GetSolution(orgId: orgId);
      var claim = Creator.GetClaimsBase(solnId: soln.Id);
      var ctx = Creator.GetContext();
      _context.Setup(c => c.HttpContext).Returns(ctx);
      _solutionDatastore.Setup(x => x.ById(soln.Id)).Returns(soln);

      var res = filter.FilterForSupplier(claim);

      res.Should().BeNull();
    }

    private DummyClaimsFilterBase Create()
    {
      return new DummyClaimsFilterBase(
        _context.Object,
        _solutionDatastore.Object,
        _solutionsFilter.Object);
    }
  }
}

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
  public sealed class TechnicalContactsFilter_Tests
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
      var ctx = Creator.GetContext(role: Roles.Admin);
      _context.Setup(c => c.HttpContext).Returns(ctx);
      var filter = Create();
      var soln = Creator.GetSolution(status: status);
      _solutionDatastore.Setup(x => x.ById(soln.Id)).Returns(soln);
      var techConts = new[]
      {
        Creator.GetTechnicalContact(solutionId: soln.Id),
        Creator.GetTechnicalContact(solutionId: soln.Id),
        Creator.GetTechnicalContact(solutionId: soln.Id)
      };
      var expTechConts = techConts.Where(x => true);

      var res = filter.Filter(techConts);

      res.Should().BeEquivalentTo(expTechConts);
    }

    [Test]
    public void Filter_Buyer_Returns_NonFailedDraft(
      [ValueSource(typeof(Creator), nameof(Creator.SolutionStatuses))]SolutionStatus status)
    {
      var ctx = Creator.GetContext(role: Roles.Buyer);
      _context.Setup(c => c.HttpContext).Returns(ctx);
      var filter = Create();
      var soln = Creator.GetSolution(status: status);
      _solutionDatastore.Setup(x => x.ById(soln.Id)).Returns(soln);
      _solutionsFilter
        .Setup(x => x.Filter(It.Is<IEnumerable<Solutions>>(solns => solns.Contains(soln))))
        .Returns(
          soln.Status != SolutionStatus.Draft &&
          soln.Status != SolutionStatus.Failed ?
            new[] { soln } : Enumerable.Empty<Solutions>());
      var techConts = new[]
      {
        Creator.GetTechnicalContact(solutionId: soln.Id),
        Creator.GetTechnicalContact(solutionId: soln.Id),
        Creator.GetTechnicalContact(solutionId: soln.Id)
      };
      var expTechConts = techConts.Where(x =>
        soln.Status != SolutionStatus.Draft &&
        soln.Status != SolutionStatus.Failed);

      var res = filter.Filter(techConts);

      res.Should().BeEquivalentTo(expTechConts);
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
      _solutionsFilter
        .Setup(x => x.Filter(It.Is<IEnumerable<Solutions>>(solns => solns.Contains(soln))))
        .Returns(soln.Status == SolutionStatus.Approved ? new[] { soln } : Enumerable.Empty<Solutions>());
      var techConts = new[]
      {
        Creator.GetTechnicalContact(solutionId: soln.Id),
        Creator.GetTechnicalContact(solutionId: soln.Id),
        Creator.GetTechnicalContact(solutionId: soln.Id)
      };
      var expTechConts = techConts.Where(x => soln.Status == SolutionStatus.Approved);

      var res = filter.Filter(techConts);

      res.Should().BeEquivalentTo(expTechConts);
    }

    [Test]
    public void Filter_Supplier_Returns_Own(
      [ValueSource(typeof(Creator), nameof(Creator.SolutionStatuses))]SolutionStatus status)
    {
      var orgId = Guid.NewGuid().ToString();
      var ctx = Creator.GetContext(orgId: orgId, role: Roles.Supplier);
      _context.Setup(c => c.HttpContext).Returns(ctx);
      var filter = Create();
      var soln1 = Creator.GetSolution(orgId: orgId, status: status);
      var soln2 = Creator.GetSolution(status: status);
      var soln3 = Creator.GetSolution(status: status);
      _solutionDatastore.Setup(x => x.ById(soln1.Id)).Returns(soln1);
      _solutionDatastore.Setup(x => x.ById(soln2.Id)).Returns(soln2);
      _solutionDatastore.Setup(x => x.ById(soln3.Id)).Returns(soln3);
      var techContCtx1 = Creator.GetTechnicalContact(solutionId: soln1.Id);
      var techContCtx2 = Creator.GetTechnicalContact(solutionId: soln2.Id);
      var techContCtx3 = Creator.GetTechnicalContact(solutionId: soln3.Id);
      var techContCtxs = new[] { techContCtx1, techContCtx2, techContCtx3 };

      var res = filter.Filter(techContCtxs);

      res.Should().BeEquivalentTo(new[] { techContCtx1 });
    }

    private TechnicalContactsFilter Create()
    {
      return new TechnicalContactsFilter(
        _context.Object,
        _solutionDatastore.Object,
        _solutionsFilter.Object);
    }
  }
}

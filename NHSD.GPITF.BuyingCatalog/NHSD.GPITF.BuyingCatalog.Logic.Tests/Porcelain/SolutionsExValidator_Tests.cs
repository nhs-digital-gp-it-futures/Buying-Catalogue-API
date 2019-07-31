using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using NHSD.GPITF.BuyingCatalog.Interfaces.Porcelain;
using NHSD.GPITF.BuyingCatalog.Logic.Porcelain;
using NHSD.GPITF.BuyingCatalog.Models;
using NHSD.GPITF.BuyingCatalog.Tests;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NHSD.GPITF.BuyingCatalog.Logic.Tests.Porcelain
{
  [TestFixture]
  public sealed class SolutionsExValidator_Tests
  {
    private Mock<IHttpContextAccessor> _context;
    private Mock<ILogger<SolutionsExValidator>> _logger;
    private Mock<ISolutionsExDatastore> _datastore;
    private Mock<ISolutionsValidator> _solutionsValidator;

    [SetUp]
    public void SetUp()
    {
      _context = new Mock<IHttpContextAccessor>();
      _logger = new Mock<ILogger<SolutionsExValidator>>();
      _datastore = new Mock<ISolutionsExDatastore>();
      _solutionsValidator = new Mock<ISolutionsValidator>();
    }

    [Test]
    public void Constructor_Completes()
    {
      Assert.DoesNotThrow(() => GetValidator());
    }

    [Test]
    public void ClaimedCapabilityMustBelongToSolution_Valid_Succeeds()
    {
      var validator = GetValidator();
      var soln = Creator.GetSolutionEx();
      soln.ClaimedCapability = new List<CapabilitiesImplemented>
      (
        new[]
        {
          Creator.GetCapabilitiesImplemented(solnId: soln.Solution.Id),
          Creator.GetCapabilitiesImplemented(solnId: soln.Solution.Id),
          Creator.GetCapabilitiesImplemented(solnId: soln.Solution.Id)
        }
      );

      validator.ClaimedCapabilityMustBelongToSolution();
      var valres = validator.Validate(soln);

      valres.Errors.Should().BeEmpty();
    }

    [Test]
    public void ClaimedCapabilityMustBelongToSolution_Invalid_ReturnsError()
    {
      var validator = GetValidator();
      var soln = Creator.GetSolutionEx();
      soln.ClaimedCapability = new List<CapabilitiesImplemented>
      (
        new[]
        {
          Creator.GetCapabilitiesImplemented(),
          Creator.GetCapabilitiesImplemented(),
          Creator.GetCapabilitiesImplemented()
        }
      );

      validator.ClaimedCapabilityMustBelongToSolution();
      var valres = validator.Validate(soln);

      valres.Errors.Should()
        .Contain(x => x.ErrorMessage == "ClaimedCapability must belong to solution")
        .And
        .HaveCount(1);
    }

    [Test]
    public void ClaimedStandardMustBelongToSolution_Valid_Succeeds()
    {
      var validator = GetValidator();
      var soln = Creator.GetSolutionEx();
      soln.ClaimedStandard = new List<StandardsApplicable>
      (
        new[]
        {
          Creator.GetStandardsApplicable(solnId: soln.Solution.Id),
          Creator.GetStandardsApplicable(solnId: soln.Solution.Id),
          Creator.GetStandardsApplicable(solnId: soln.Solution.Id)
        }
      );

      validator.ClaimedStandardMustBelongToSolution();
      var valres = validator.Validate(soln);

      valres.Errors.Should().BeEmpty();
    }

    [Test]
    public void ClaimedStandardMustBelongToSolution_Invalid_ReturnsError()
    {
      var validator = GetValidator();
      var soln = Creator.GetSolutionEx();
      soln.ClaimedStandard = new List<StandardsApplicable>
      (
        new[]
        {
          Creator.GetStandardsApplicable(),
          Creator.GetStandardsApplicable(),
          Creator.GetStandardsApplicable()
        }
      );

      validator.ClaimedStandardMustBelongToSolution();
      var valres = validator.Validate(soln);

      valres.Errors.Should()
        .Contain(x => x.ErrorMessage == "ClaimedStandard must belong to solution")
        .And
        .HaveCount(1);
    }

    [Test]
    public void TechnicalContactMustBelongToSolution_Valid_Succeeds()
    {
      var validator = GetValidator();
      var soln = Creator.GetSolutionEx();
      var techCont = Creator.GetTechnicalContact(solutionId: soln.Solution.Id);
      soln.TechnicalContact = new List<TechnicalContacts>(new[] { techCont });

      validator.TechnicalContactMustBelongToSolution();
      var valres = validator.Validate(soln);

      valres.Errors.Should().BeEmpty();
    }

    [Test]
    public void TechnicalContactMustBelongToSolution_Invalid_ReturnsError()
    {
      var validator = GetValidator();
      var soln = Creator.GetSolutionEx();
      var techCont = Creator.GetTechnicalContact();
      soln.TechnicalContact = new List<TechnicalContacts>(new[] { techCont });

      validator.TechnicalContactMustBelongToSolution();
      var valres = validator.Validate(soln);

      valres.Errors.Should()
        .Contain(x => x.ErrorMessage == "TechnicalContact must belong to solution")
        .And
        .HaveCount(1);
    }

    #region Claims
    public static IEnumerable<SolutionStatus> SolutionStatusesPendingForClaims()
    {
      yield return SolutionStatus.Draft;
      yield return SolutionStatus.Registered;
      yield return SolutionStatus.CapabilitiesAssessment;
      yield return SolutionStatus.StandardsCompliance;
    }

    public static IEnumerable<SolutionStatus> SolutionStatusesNotPendingForClaims()
    {
      return Creator.SolutionStatuses().Except(SolutionStatusesPendingForClaims());
    }

    [Test]
    public void MustBePendingToChangeClaimedCapability_SameCapability_Succeeds(
      [ValueSource(typeof(Creator), nameof(Creator.SolutionStatuses))]SolutionStatus status)
    {
      var validator = GetValidator();

      var oldSolnEx = Creator.GetSolutionEx();
      var oldQual1 = Creator.GetCapabilitiesImplemented();
      var oldQual2 = Creator.GetCapabilitiesImplemented();
      oldSolnEx.ClaimedCapability = new List<CapabilitiesImplemented>(new[] { oldQual1, oldQual2 });

      var newSoln = Creator.GetSolution(status: status);
      var newSolnEx = Creator.GetSolutionEx(soln: newSoln);
      newSolnEx.ClaimedCapability = new List<CapabilitiesImplemented>(new[] { oldQual1.Clone(), oldQual2.Clone() });


      var res = validator.MustBePendingToChangeClaimedCapability(oldSolnEx, newSolnEx);


      res.Should().BeTrue();
    }

    [Test]
    public void MustBePendingToChangeClaimedCapability_NotPending_DifferentCapability_Fails(
      [ValueSource(nameof(SolutionStatusesNotPendingForClaims))]SolutionStatus status)
    {
      var validator = GetValidator();

      var oldSolnEx = Creator.GetSolutionEx();
      var oldQual1 = Creator.GetCapabilitiesImplemented();
      var oldQual2 = Creator.GetCapabilitiesImplemented();
      oldSolnEx.ClaimedCapability = new List<CapabilitiesImplemented>(new[] { oldQual1, oldQual2 });

      var newSoln = Creator.GetSolution(status: status);
      var newSolnEx = Creator.GetSolutionEx(soln: newSoln);
      var newQual1 = Creator.GetCapabilitiesImplemented();
      var newQual2 = Creator.GetCapabilitiesImplemented();
      newSolnEx.ClaimedCapability = new List<CapabilitiesImplemented>(new[] { newQual1, newQual2 });


      var res = validator.MustBePendingToChangeClaimedCapability(oldSolnEx, newSolnEx);


      res.Should().BeFalse();
    }

    [Test]
    public void MustBePendingToChangeClaimedCapability_Pending_DifferentCapability_Succeeds(
      [ValueSource(nameof(SolutionStatusesPendingForClaims))]SolutionStatus status)
    {
      var validator = GetValidator();

      var oldSolnEx = Creator.GetSolutionEx();
      var oldQual1 = Creator.GetCapabilitiesImplemented();
      var oldQual2 = Creator.GetCapabilitiesImplemented();
      oldSolnEx.ClaimedCapability = new List<CapabilitiesImplemented>(new[] { oldQual1, oldQual2 });

      var newSoln = Creator.GetSolution(status: status);
      var newSolnEx = Creator.GetSolutionEx(soln: newSoln);
      var newQual1 = Creator.GetCapabilitiesImplemented();
      var newQual2 = Creator.GetCapabilitiesImplemented();
      newSolnEx.ClaimedCapability = new List<CapabilitiesImplemented>(new[] { newQual1, newQual2 });


      var res = validator.MustBePendingToChangeClaimedCapability(oldSolnEx, newSolnEx);


      res.Should().BeTrue();
    }

    [Test]
    public void MustBePendingToChangeClaimedStandard_SameStandard_Succeeds(
      [ValueSource(typeof(Creator), nameof(Creator.SolutionStatuses))]SolutionStatus status)
    {
      var validator = GetValidator();

      var oldSolnEx = Creator.GetSolutionEx();
      var oldQual1 = Creator.GetStandardsApplicable();
      var oldQual2 = Creator.GetStandardsApplicable();
      oldSolnEx.ClaimedStandard = new List<StandardsApplicable>(new[] { oldQual1, oldQual2 });

      var newSoln = Creator.GetSolution(status: status);
      var newSolnEx = Creator.GetSolutionEx(soln: newSoln);
      newSolnEx.ClaimedStandard = new List<StandardsApplicable>(new[] { oldQual1.Clone(), oldQual2.Clone() });


      var res = validator.MustBePendingToChangeClaimedStandard(oldSolnEx, newSolnEx);


      res.Should().BeTrue();
    }

    [Test]
    public void MustBePendingToChangeClaimedStandard_NotPending_DifferentStandard_Fails(
      [ValueSource(nameof(SolutionStatusesNotPendingForClaims))]SolutionStatus status)
    {
      var validator = GetValidator();

      var oldSolnEx = Creator.GetSolutionEx();
      var oldQual1 = Creator.GetStandardsApplicable();
      var oldQual2 = Creator.GetStandardsApplicable();
      oldSolnEx.ClaimedStandard = new List<StandardsApplicable>(new[] { oldQual1, oldQual2 });

      var newSoln = Creator.GetSolution(status: status);
      var newSolnEx = Creator.GetSolutionEx(soln: newSoln);
      var newQual1 = Creator.GetStandardsApplicable();
      var newQual2 = Creator.GetStandardsApplicable();
      newSolnEx.ClaimedStandard = new List<StandardsApplicable>(new[] { newQual1, newQual2 });


      var res = validator.MustBePendingToChangeClaimedStandard(oldSolnEx, newSolnEx);


      res.Should().BeFalse();
    }

    [Test]
    public void MustBePendingToChangeClaimedStandard_Pending_DifferentStandard_Succeeds(
      [ValueSource(nameof(SolutionStatusesPendingForClaims))]SolutionStatus status)
    {
      var validator = GetValidator();

      var oldSolnEx = Creator.GetSolutionEx();
      var oldQual1 = Creator.GetStandardsApplicable();
      var oldQual2 = Creator.GetStandardsApplicable();
      oldSolnEx.ClaimedStandard = new List<StandardsApplicable>(new[] { oldQual1, oldQual2 });

      var newSoln = Creator.GetSolution(status: status);
      var newSolnEx = Creator.GetSolutionEx(soln: newSoln);
      var newQual1 = Creator.GetStandardsApplicable();
      var newQual2 = Creator.GetStandardsApplicable();
      newSolnEx.ClaimedStandard = new List<StandardsApplicable>(new[] { newQual1, newQual2 });


      var res = validator.MustBePendingToChangeClaimedStandard(oldSolnEx, newSolnEx);


      res.Should().BeTrue();
    }
    #endregion

    private SolutionsExValidator GetValidator()
    {
      return new SolutionsExValidator(_context.Object, _logger.Object, _datastore.Object, _solutionsValidator.Object);
    }
  }
}

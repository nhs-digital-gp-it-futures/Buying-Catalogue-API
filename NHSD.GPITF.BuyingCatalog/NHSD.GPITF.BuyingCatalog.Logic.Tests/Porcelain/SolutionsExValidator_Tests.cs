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
    public void ClaimedCapabilityEvidenceMustBelongToClaim_Valid_Succeeds()
    {
      var validator = GetValidator();
      var soln = Creator.GetSolutionEx();
      var claim = Creator.GetCapabilitiesImplemented();
      var claimEv = Creator.GetCapabilitiesImplementedEvidence(claimId: claim.Id);
      soln.ClaimedCapability = new List<CapabilitiesImplemented>(new[] { claim });
      soln.ClaimedCapabilityEvidence = new List<CapabilitiesImplementedEvidence>(new[] { claimEv });

      validator.ClaimedCapabilityEvidenceMustBelongToClaim();
      var valres = validator.Validate(soln);

      valres.Errors.Should().BeEmpty();
    }

    [Test]
    public void ClaimedCapabilityEvidenceMustBelongToClaim_Invalid_ReturnsError()
    {
      var validator = GetValidator();
      var soln = Creator.GetSolutionEx();
      var claim = Creator.GetCapabilitiesImplemented();
      var claimEv = Creator.GetCapabilitiesImplementedEvidence();
      soln.ClaimedCapability = new List<CapabilitiesImplemented>(new[] { claim });
      soln.ClaimedCapabilityEvidence = new List<CapabilitiesImplementedEvidence>(new[] { claimEv });

      validator.ClaimedCapabilityEvidenceMustBelongToClaim();
      var valres = validator.Validate(soln);

      valres.Errors.Should()
        .Contain(x => x.ErrorMessage == "ClaimedCapabilityEvidence must belong to claim")
        .And
        .HaveCount(1);
    }

    [Test]
    public void ClaimedStandardEvidenceMustBelongToClaim_Valid_Succeeds()
    {
      var validator = GetValidator();
      var soln = Creator.GetSolutionEx();
      var claim = Creator.GetStandardsApplicable();
      var claimEv = Creator.GetStandardsApplicableEvidence(claimId: claim.Id);
      soln.ClaimedStandard = new List<StandardsApplicable>(new[] { claim });
      soln.ClaimedStandardEvidence = new List<StandardsApplicableEvidence>(new[] { claimEv });

      validator.ClaimedStandardEvidenceMustBelongToClaim();
      var valres = validator.Validate(soln);

      valres.Errors.Should().BeEmpty();
    }

    [Test]
    public void ClaimedStandardEvidenceMustBelongToClaim_Invalid_ReturnsError()
    {
      var validator = GetValidator();
      var soln = Creator.GetSolutionEx();
      var claim = Creator.GetStandardsApplicable();
      var claimEv = Creator.GetStandardsApplicableEvidence();
      soln.ClaimedStandard = new List<StandardsApplicable>(new[] { claim });
      soln.ClaimedStandardEvidence = new List<StandardsApplicableEvidence>(new[] { claimEv });

      validator.ClaimedStandardEvidenceMustBelongToClaim();
      var valres = validator.Validate(soln);

      valres.Errors.Should()
        .Contain(x => x.ErrorMessage == "ClaimedStandardEvidence must belong to claim")
        .And
        .HaveCount(1);
    }

    [Test]
    public void ClaimedCapabilityReviewMustBelongToEvidence_Valid_Succeeds()
    {
      var validator = GetValidator();
      var soln = Creator.GetSolutionEx();
      var claimEv = Creator.GetCapabilitiesImplementedEvidence();
      var review = Creator.GetCapabilitiesImplementedReviews(evidenceId: claimEv.Id);
      soln.ClaimedCapabilityEvidence = new List<CapabilitiesImplementedEvidence>(new[] { claimEv });
      soln.ClaimedCapabilityReview = new List<CapabilitiesImplementedReviews>(new[] { review });

      validator.ClaimedCapabilityReviewMustBelongToEvidence();
      var valres = validator.Validate(soln);

      valres.Errors.Should().BeEmpty();
    }

    [Test]
    public void ClaimedCapabilityReviewMustBelongToEvidence_Invalid_ReturnsError()
    {
      var validator = GetValidator();
      var soln = Creator.GetSolutionEx();
      var claimEv = Creator.GetCapabilitiesImplementedEvidence();
      var review = Creator.GetCapabilitiesImplementedReviews();
      soln.ClaimedCapabilityEvidence = new List<CapabilitiesImplementedEvidence>(new[] { claimEv });
      soln.ClaimedCapabilityReview = new List<CapabilitiesImplementedReviews>(new[] { review });

      validator.ClaimedCapabilityReviewMustBelongToEvidence();
      var valres = validator.Validate(soln);

      valres.Errors.Should()
        .Contain(x => x.ErrorMessage == "ClaimedCapabilityReview must belong to evidence")
        .And
        .HaveCount(1);
    }

    [Test]
    public void ClaimedStandardReviewMustBelongToEvidence_Valid_Succeeds()
    {
      var validator = GetValidator();
      var soln = Creator.GetSolutionEx();
      var claimEv = Creator.GetStandardsApplicableEvidence();
      var review = Creator.GetStandardsApplicableReviews(evidenceId: claimEv.Id);
      soln.ClaimedStandardEvidence = new List<StandardsApplicableEvidence>(new[] { claimEv });
      soln.ClaimedStandardReview = new List<StandardsApplicableReviews>(new[] { review });

      validator.ClaimedStandardReviewMustBelongToEvidence();
      var valres = validator.Validate(soln);

      valres.Errors.Should().BeEmpty();
    }

    [Test]
    public void ClaimedStandardReviewMustBelongToEvidence_Invalid_ReturnsError()
    {
      var validator = GetValidator();
      var soln = Creator.GetSolutionEx();
      var claimEv = Creator.GetStandardsApplicableEvidence();
      var review = Creator.GetStandardsApplicableReviews();
      soln.ClaimedStandardEvidence = new List<StandardsApplicableEvidence>(new[] { claimEv });
      soln.ClaimedStandardReview = new List<StandardsApplicableReviews>(new[] { review });

      validator.ClaimedStandardReviewMustBelongToEvidence();
      var valres = validator.Validate(soln);

      valres.Errors.Should()
        .Contain(x => x.ErrorMessage == "ClaimedStandardReview must belong to evidence")
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

    [Test]
    public void ClaimedCapabilityEvidencePreviousVersionMustBelongToSolution_Valid_Succeeds()
    {
      var validator = GetValidator();
      var soln = Creator.GetSolutionEx();
      var claimEv1 = Creator.GetCapabilitiesImplementedEvidence();
      var claimEv2 = Creator.GetCapabilitiesImplementedEvidence(prevId: claimEv1.Id);
      soln.ClaimedCapabilityEvidence = new List<CapabilitiesImplementedEvidence>(new[] { claimEv1, claimEv2 });

      validator.ClaimedCapabilityEvidencePreviousVersionMustBelongToSolution();
      var valres = validator.Validate(soln);

      valres.Errors.Should().BeEmpty();
    }

    [Test]
    public void ClaimedCapabilityEvidencePreviousVersionMustBelongToSolution_Invalid_ReturnsError()
    {
      var validator = GetValidator();
      var soln = Creator.GetSolutionEx();
      var claimEv1 = Creator.GetCapabilitiesImplementedEvidence();
      var claimEv2 = Creator.GetCapabilitiesImplementedEvidence(prevId: Guid.NewGuid().ToString());
      soln.ClaimedCapabilityEvidence = new List<CapabilitiesImplementedEvidence>(new[] { claimEv1, claimEv2 });

      validator.ClaimedCapabilityEvidencePreviousVersionMustBelongToSolution();
      var valres = validator.Validate(soln);

      valres.Errors.Should()
        .Contain(x => x.ErrorMessage == "ClaimedCapabilityEvidence previous version must belong to solution")
        .And
        .HaveCount(1);
    }

    [Test]
    public void ClaimedStandardEvidencePreviousVersionMustBelongToSolution_Valid_Succeeds()
    {
      var validator = GetValidator();
      var soln = Creator.GetSolutionEx();
      var claimEv1 = Creator.GetStandardsApplicableEvidence();
      var claimEv2 = Creator.GetStandardsApplicableEvidence(prevId: claimEv1.Id);
      soln.ClaimedStandardEvidence = new List<StandardsApplicableEvidence>(new[] { claimEv1, claimEv2 });

      validator.ClaimedStandardEvidencePreviousVersionMustBelongToSolution();
      var valres = validator.Validate(soln);

      valres.Errors.Should().BeEmpty();
    }

    [Test]
    public void ClaimedStandardEvidencePreviousVersionMustBelongToSolution_Invalid_ReturnsError()
    {
      var validator = GetValidator();
      var soln = Creator.GetSolutionEx();
      var claimEv1 = Creator.GetStandardsApplicableEvidence();
      var claimEv2 = Creator.GetStandardsApplicableEvidence(prevId: Guid.NewGuid().ToString());
      soln.ClaimedStandardEvidence = new List<StandardsApplicableEvidence>(new[] { claimEv1, claimEv2 });

      validator.ClaimedStandardEvidencePreviousVersionMustBelongToSolution();
      var valres = validator.Validate(soln);

      valres.Errors.Should()
        .Contain(x => x.ErrorMessage == "ClaimedStandardEvidence previous version must belong to solution")
        .And
        .HaveCount(1);
    }

    [Test]
    public void ClaimedCapabilityReviewPreviousVersionMustBelongToSolution_Valid_Succeeds()
    {
      var validator = GetValidator();
      var soln = Creator.GetSolutionEx();
      var review1 = Creator.GetCapabilitiesImplementedReviews();
      var review2 = Creator.GetCapabilitiesImplementedReviews(prevId: review1.Id);
      soln.ClaimedCapabilityReview = new List<CapabilitiesImplementedReviews>(new[] { review1, review2 });

      validator.ClaimedCapabilityReviewPreviousVersionMustBelongToSolution();
      var valres = validator.Validate(soln);

      valres.Errors.Should().BeEmpty();
    }

    [Test]
    public void ClaimedCapabilityReviewPreviousVersionMustBelongToSolution_Invalid_ReturnsError()
    {
      var validator = GetValidator();
      var soln = Creator.GetSolutionEx();
      var review1 = Creator.GetCapabilitiesImplementedReviews();
      var review2 = Creator.GetCapabilitiesImplementedReviews(prevId: Guid.NewGuid().ToString());
      soln.ClaimedCapabilityReview = new List<CapabilitiesImplementedReviews>(new[] { review1, review2 });

      validator.ClaimedCapabilityReviewPreviousVersionMustBelongToSolution();
      var valres = validator.Validate(soln);

      valres.Errors.Should()
        .Contain(x => x.ErrorMessage == "ClaimedCapabilityReview previous version must belong to solution")
        .And
        .HaveCount(1);
    }

    [Test]
    public void ClaimedStandardReviewPreviousVersionMustBelongToSolution_Valid_Succeeds()
    {
      var validator = GetValidator();
      var soln = Creator.GetSolutionEx();
      var review1 = Creator.GetStandardsApplicableReviews();
      var review2 = Creator.GetStandardsApplicableReviews(prevId: review1.Id);
      soln.ClaimedStandardReview = new List<StandardsApplicableReviews>(new[] { review1, review2 });

      validator.ClaimedStandardReviewPreviousVersionMustBelongToSolution();
      var valres = validator.Validate(soln);

      valres.Errors.Should().BeEmpty();
    }

    [Test]
    public void ClaimedStandardReviewPreviousVersionMustBelongToSolution_Invalid_ReturnsError()
    {
      var validator = GetValidator();
      var soln = Creator.GetSolutionEx();
      var review1 = Creator.GetStandardsApplicableReviews();
      var review2 = Creator.GetStandardsApplicableReviews(prevId: Guid.NewGuid().ToString());
      soln.ClaimedStandardReview = new List<StandardsApplicableReviews>(new[] { review1, review2 });

      validator.ClaimedStandardReviewPreviousVersionMustBelongToSolution();
      var valres = validator.Validate(soln);

      valres.Errors.Should()
        .Contain(x => x.ErrorMessage == "ClaimedStandardReview previous version must belong to solution")
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

    #region Evidence
    public static IEnumerable<SolutionStatus> SolutionStatusesPendingForEvidence()
    {
      yield return SolutionStatus.Registered;
      yield return SolutionStatus.CapabilitiesAssessment;
      yield return SolutionStatus.StandardsCompliance;
    }

    public static IEnumerable<SolutionStatus> SolutionStatusesNotPendingForEvidence()
    {
      return Creator.SolutionStatuses().Except(SolutionStatusesPendingForEvidence());
    }

    #region ClaimedCapabilityEvidence
    [Test]
    public void MustBePendingToChangeClaimedCapabilityEvidence_SameEvidence_Succeeds(
      [ValueSource(typeof(Creator), nameof(Creator.SolutionStatuses))]SolutionStatus status)
    {
      var validator = GetValidator();

      var oldSolnEx = Creator.GetSolutionEx();
      var oldEv1 = Creator.GetCapabilitiesImplementedEvidence();
      var oldEv2 = Creator.GetCapabilitiesImplementedEvidence();
      oldSolnEx.ClaimedCapabilityEvidence = new List<CapabilitiesImplementedEvidence>(new[] { oldEv1, oldEv2 });

      var newSoln = Creator.GetSolution(status: status);
      var newSolnEx = Creator.GetSolutionEx(soln: newSoln);
      newSolnEx.ClaimedCapabilityEvidence = new List<CapabilitiesImplementedEvidence>(new[] { oldEv1.Clone(), oldEv2.Clone() });


      var res = validator.MustBePendingToChangeClaimedCapabilityEvidence(oldSolnEx, newSolnEx);


      res.Should().BeTrue();
    }

    [Test]
    public void MustBePendingToChangeClaimedCapabilityEvidence_Pending_AddEvidence_Succeeds(
      [ValueSource(nameof(SolutionStatusesPendingForEvidence))]SolutionStatus status)
    {
      var validator = GetValidator();

      var oldSolnEx = Creator.GetSolutionEx();
      var oldEv1 = Creator.GetCapabilitiesImplementedEvidence();
      var oldEv2 = Creator.GetCapabilitiesImplementedEvidence();
      oldSolnEx.ClaimedCapabilityEvidence = new List<CapabilitiesImplementedEvidence>(new[] { oldEv1, oldEv2 });

      var newSoln = Creator.GetSolution(status: status);
      var newSolnEx = Creator.GetSolutionEx(soln: newSoln);
      var newEv3 = Creator.GetCapabilitiesImplementedEvidence();
      newSolnEx.ClaimedCapabilityEvidence = new List<CapabilitiesImplementedEvidence>(new[] { oldEv1.Clone(), oldEv2.Clone(), newEv3 });


      var res = validator.MustBePendingToChangeClaimedCapabilityEvidence(oldSolnEx, newSolnEx);


      res.Should().BeTrue();
    }

    [Test]
    public void MustBePendingToChangeClaimedCapabilityEvidence_NotPending_AddEvidence_Fails(
      [ValueSource(nameof(SolutionStatusesNotPendingForEvidence))]SolutionStatus status)
    {
      var validator = GetValidator();

      var oldSolnEx = Creator.GetSolutionEx();
      var oldEv1 = Creator.GetCapabilitiesImplementedEvidence();
      var oldEv2 = Creator.GetCapabilitiesImplementedEvidence();
      oldSolnEx.ClaimedCapabilityEvidence = new List<CapabilitiesImplementedEvidence>(new[] { oldEv1, oldEv2 });

      var newSoln = Creator.GetSolution(status: status);
      var newSolnEx = Creator.GetSolutionEx(soln: newSoln);
      var newEv3 = Creator.GetCapabilitiesImplementedEvidence();
      newSolnEx.ClaimedCapabilityEvidence = new List<CapabilitiesImplementedEvidence>(new[] { oldEv1.Clone(), oldEv2.Clone(), newEv3 });


      var res = validator.MustBePendingToChangeClaimedCapabilityEvidence(oldSolnEx, newSolnEx);


      res.Should().BeFalse();
    }

    [Test]
    public void MustBePendingToChangeClaimedCapabilityEvidence_Pending_DifferentEvidence_Succeeds(
      [ValueSource(nameof(SolutionStatusesPendingForEvidence))]SolutionStatus status)
    {
      var validator = GetValidator();

      var oldSolnEx = Creator.GetSolutionEx();
      var oldEv1 = Creator.GetCapabilitiesImplementedEvidence();
      var oldEv2 = Creator.GetCapabilitiesImplementedEvidence();
      oldSolnEx.ClaimedCapabilityEvidence = new List<CapabilitiesImplementedEvidence>(new[] { oldEv1, oldEv2 });

      var newSoln = Creator.GetSolution(status: status);
      var newSolnEx = Creator.GetSolutionEx(soln: newSoln);
      var newEv1 = Creator.GetCapabilitiesImplementedEvidence();
      var newEv2 = Creator.GetCapabilitiesImplementedEvidence();
      newSolnEx.ClaimedCapabilityEvidence = new List<CapabilitiesImplementedEvidence>(new[] { newEv1, newEv2 });


      var res = validator.MustBePendingToChangeClaimedCapabilityEvidence(oldSolnEx, newSolnEx);


      res.Should().BeTrue();
    }

    [Test]
    public void MustBePendingToChangeClaimedCapabilityEvidence_NotPending_DifferentEvidence_Fails(
      [ValueSource(nameof(SolutionStatusesNotPendingForEvidence))]SolutionStatus status)
    {
      var validator = GetValidator();

      var oldSolnEx = Creator.GetSolutionEx();
      var oldEv1 = Creator.GetCapabilitiesImplementedEvidence();
      var oldEv2 = Creator.GetCapabilitiesImplementedEvidence();
      oldSolnEx.ClaimedCapabilityEvidence = new List<CapabilitiesImplementedEvidence>(new[] { oldEv1, oldEv2 });

      var newSoln = Creator.GetSolution(status: status);
      var newSolnEx = Creator.GetSolutionEx(soln: newSoln);
      var newEv1 = Creator.GetCapabilitiesImplementedEvidence();
      var newEv2 = Creator.GetCapabilitiesImplementedEvidence();
      newSolnEx.ClaimedCapabilityEvidence = new List<CapabilitiesImplementedEvidence>(new[] { newEv1, newEv2 });


      var res = validator.MustBePendingToChangeClaimedCapabilityEvidence(oldSolnEx, newSolnEx);


      res.Should().BeFalse();
    }

    [Test]
    public void MustBePendingToChangeClaimedCapabilityEvidence_Pending_RemoveEvidence_Succeeds(
      [ValueSource(nameof(SolutionStatusesPendingForEvidence))]SolutionStatus status)
    {
      var validator = GetValidator();

      var oldSolnEx = Creator.GetSolutionEx();
      var oldEv1 = Creator.GetCapabilitiesImplementedEvidence();
      var oldEv2 = Creator.GetCapabilitiesImplementedEvidence();
      oldSolnEx.ClaimedCapabilityEvidence = new List<CapabilitiesImplementedEvidence>(new[] { oldEv1, oldEv2 });

      var newSoln = Creator.GetSolution(status: status);
      var newSolnEx = Creator.GetSolutionEx(soln: newSoln);
      newSolnEx.ClaimedCapabilityEvidence = new List<CapabilitiesImplementedEvidence>(new[] { oldEv1.Clone() });


      var res = validator.MustBePendingToChangeClaimedCapabilityEvidence(oldSolnEx, newSolnEx);


      res.Should().BeTrue();
    }

    [Test]
    public void MustBePendingToChangeClaimedCapabilityEvidence_NotPending_RemoveEvidence_Fails(
      [ValueSource(nameof(SolutionStatusesNotPendingForEvidence))]SolutionStatus status)
    {
      var validator = GetValidator();

      var oldSolnEx = Creator.GetSolutionEx();
      var oldEv1 = Creator.GetCapabilitiesImplementedEvidence();
      var oldEv2 = Creator.GetCapabilitiesImplementedEvidence();
      oldSolnEx.ClaimedCapabilityEvidence = new List<CapabilitiesImplementedEvidence>(new[] { oldEv1, oldEv2 });

      var newSoln = Creator.GetSolution(status: status);
      var newSolnEx = Creator.GetSolutionEx(soln: newSoln);
      newSolnEx.ClaimedCapabilityEvidence = new List<CapabilitiesImplementedEvidence>(new[] { oldEv1.Clone() });


      var res = validator.MustBePendingToChangeClaimedCapabilityEvidence(oldSolnEx, newSolnEx);


      res.Should().BeFalse();
    }
    #endregion

    #region ClaimedStandardEvidence
    [Test]
    public void MustBePendingToChangeClaimedStandardEvidence_SameEvidence_Succeeds(
      [ValueSource(typeof(Creator), nameof(Creator.SolutionStatuses))]SolutionStatus status)
    {
      var validator = GetValidator();

      var oldSolnEx = Creator.GetSolutionEx();
      var oldEv1 = Creator.GetStandardsApplicableEvidence();
      var oldEv2 = Creator.GetStandardsApplicableEvidence();
      oldSolnEx.ClaimedStandardEvidence = new List<StandardsApplicableEvidence>(new[] { oldEv1, oldEv2 });

      var newSoln = Creator.GetSolution(status: status);
      var newSolnEx = Creator.GetSolutionEx(soln: newSoln);
      newSolnEx.ClaimedStandardEvidence = new List<StandardsApplicableEvidence>(new[] { oldEv1.Clone(), oldEv2.Clone() });


      var res = validator.MustBePendingToChangeClaimedStandardEvidence(oldSolnEx, newSolnEx);


      res.Should().BeTrue();
    }

    [Test]
    public void MustBePendingToChangeClaimedStandardEvidence_Pending_AddEvidence_Succeeds(
      [ValueSource(nameof(SolutionStatusesPendingForEvidence))]SolutionStatus status)
    {
      var validator = GetValidator();

      var oldSolnEx = Creator.GetSolutionEx();
      var oldEv1 = Creator.GetStandardsApplicableEvidence();
      var oldEv2 = Creator.GetStandardsApplicableEvidence();
      oldSolnEx.ClaimedStandardEvidence = new List<StandardsApplicableEvidence>(new[] { oldEv1, oldEv2 });

      var newSoln = Creator.GetSolution(status: status);
      var newSolnEx = Creator.GetSolutionEx(soln: newSoln);
      var newEv3 = Creator.GetStandardsApplicableEvidence();
      newSolnEx.ClaimedStandardEvidence = new List<StandardsApplicableEvidence>(new[] { oldEv1.Clone(), oldEv2.Clone(), newEv3 });


      var res = validator.MustBePendingToChangeClaimedStandardEvidence(oldSolnEx, newSolnEx);


      res.Should().BeTrue();
    }

    [Test]
    public void MustBePendingToChangeClaimedStandardEvidence_NotPending_AddEvidence_Fails(
      [ValueSource(nameof(SolutionStatusesNotPendingForEvidence))]SolutionStatus status)
    {
      var validator = GetValidator();

      var oldSolnEx = Creator.GetSolutionEx();
      var oldEv1 = Creator.GetStandardsApplicableEvidence();
      var oldEv2 = Creator.GetStandardsApplicableEvidence();
      oldSolnEx.ClaimedStandardEvidence = new List<StandardsApplicableEvidence>(new[] { oldEv1, oldEv2 });

      var newSoln = Creator.GetSolution(status: status);
      var newSolnEx = Creator.GetSolutionEx(soln: newSoln);
      var newEv3 = Creator.GetStandardsApplicableEvidence();
      newSolnEx.ClaimedStandardEvidence = new List<StandardsApplicableEvidence>(new[] { oldEv1.Clone(), oldEv2.Clone(), newEv3 });


      var res = validator.MustBePendingToChangeClaimedStandardEvidence(oldSolnEx, newSolnEx);


      res.Should().BeFalse();
    }

    [Test]
    public void MustBePendingToChangeClaimedStandardEvidence_Pending_DifferentEvidence_Succeeds(
      [ValueSource(nameof(SolutionStatusesPendingForEvidence))]SolutionStatus status)
    {
      var validator = GetValidator();

      var oldSolnEx = Creator.GetSolutionEx();
      var oldEv1 = Creator.GetStandardsApplicableEvidence();
      var oldEv2 = Creator.GetStandardsApplicableEvidence();
      oldSolnEx.ClaimedStandardEvidence = new List<StandardsApplicableEvidence>(new[] { oldEv1, oldEv2 });

      var newSoln = Creator.GetSolution(status: status);
      var newSolnEx = Creator.GetSolutionEx(soln: newSoln);
      var newEv1 = Creator.GetStandardsApplicableEvidence();
      var newEv2 = Creator.GetStandardsApplicableEvidence();
      newSolnEx.ClaimedStandardEvidence = new List<StandardsApplicableEvidence>(new[] { newEv1, newEv2 });


      var res = validator.MustBePendingToChangeClaimedStandardEvidence(oldSolnEx, newSolnEx);


      res.Should().BeTrue();
    }

    [Test]
    public void MustBePendingToChangeClaimedStandardEvidence_NotPending_DifferentEvidence_Fails(
      [ValueSource(nameof(SolutionStatusesNotPendingForEvidence))]SolutionStatus status)
    {
      var validator = GetValidator();

      var oldSolnEx = Creator.GetSolutionEx();
      var oldEv1 = Creator.GetStandardsApplicableEvidence();
      var oldEv2 = Creator.GetStandardsApplicableEvidence();
      oldSolnEx.ClaimedStandardEvidence = new List<StandardsApplicableEvidence>(new[] { oldEv1, oldEv2 });

      var newSoln = Creator.GetSolution(status: status);
      var newSolnEx = Creator.GetSolutionEx(soln: newSoln);
      var newEv1 = Creator.GetStandardsApplicableEvidence();
      var newEv2 = Creator.GetStandardsApplicableEvidence();
      newSolnEx.ClaimedStandardEvidence = new List<StandardsApplicableEvidence>(new[] { newEv1, newEv2 });


      var res = validator.MustBePendingToChangeClaimedStandardEvidence(oldSolnEx, newSolnEx);


      res.Should().BeFalse();
    }

    [Test]
    public void MustBePendingToChangeClaimedStandardEvidence_Pending_RemoveEvidence_Succeeds(
      [ValueSource(nameof(SolutionStatusesPendingForEvidence))]SolutionStatus status)
    {
      var validator = GetValidator();

      var oldSolnEx = Creator.GetSolutionEx();
      var oldEv1 = Creator.GetStandardsApplicableEvidence();
      var oldEv2 = Creator.GetStandardsApplicableEvidence();
      oldSolnEx.ClaimedStandardEvidence = new List<StandardsApplicableEvidence>(new[] { oldEv1, oldEv2 });

      var newSoln = Creator.GetSolution(status: status);
      var newSolnEx = Creator.GetSolutionEx(soln: newSoln);
      newSolnEx.ClaimedStandardEvidence = new List<StandardsApplicableEvidence>(new[] { oldEv1.Clone() });


      var res = validator.MustBePendingToChangeClaimedStandardEvidence(oldSolnEx, newSolnEx);


      res.Should().BeTrue();
    }

    [Test]
    public void MustBePendingToChangeClaimedStandardEvidence_NotPending_RemoveEvidence_Fails(
      [ValueSource(nameof(SolutionStatusesNotPendingForEvidence))]SolutionStatus status)
    {
      var validator = GetValidator();

      var oldSolnEx = Creator.GetSolutionEx();
      var oldEv1 = Creator.GetStandardsApplicableEvidence();
      var oldEv2 = Creator.GetStandardsApplicableEvidence();
      oldSolnEx.ClaimedStandardEvidence = new List<StandardsApplicableEvidence>(new[] { oldEv1, oldEv2 });

      var newSoln = Creator.GetSolution(status: status);
      var newSolnEx = Creator.GetSolutionEx(soln: newSoln);
      newSolnEx.ClaimedStandardEvidence = new List<StandardsApplicableEvidence>(new[] { oldEv1.Clone() });


      var res = validator.MustBePendingToChangeClaimedStandardEvidence(oldSolnEx, newSolnEx);


      res.Should().BeFalse();
    }
    #endregion
    #endregion

    #region Reviews
    public static IEnumerable<SolutionStatus> SolutionStatusesPendingForReview()
    {
      yield return SolutionStatus.CapabilitiesAssessment;
      yield return SolutionStatus.StandardsCompliance;
    }

    public static IEnumerable<SolutionStatus> SolutionStatusesNotPendingForReview()
    {
      return Creator.SolutionStatuses().Except(SolutionStatusesPendingForReview());
    }

    #region ClaimedCapabilityReview
    [Test]
    public void MustBePendingToChangeClaimedCapabilityReview_SameReview_Succeeds(
      [ValueSource(typeof(Creator), nameof(Creator.SolutionStatuses))]SolutionStatus status)
    {
      var validator = GetValidator();

      var oldSolnEx = Creator.GetSolutionEx();
      var oldRev1 = Creator.GetCapabilitiesImplementedReviews();
      var oldRev2 = Creator.GetCapabilitiesImplementedReviews();
      oldSolnEx.ClaimedCapabilityReview = new List<CapabilitiesImplementedReviews>(new[] { oldRev1, oldRev2 });

      var newSoln = Creator.GetSolution(status: status);
      var newSolnEx = Creator.GetSolutionEx(soln: newSoln);
      newSolnEx.ClaimedCapabilityReview = new List<CapabilitiesImplementedReviews>(new[] { oldRev1.Clone(), oldRev2.Clone() });


      var res = validator.MustBePendingToChangeClaimedCapabilityReview(oldSolnEx, newSolnEx);


      res.Should().BeTrue();
    }

    [Test]
    public void MustBePendingToChangeClaimedCapabilityReview_Pending_AddReview_Succeeds(
      [ValueSource(nameof(SolutionStatusesPendingForReview))]SolutionStatus status)
    {
      var validator = GetValidator();

      var oldSolnEx = Creator.GetSolutionEx();
      var oldRev1 = Creator.GetCapabilitiesImplementedReviews();
      var oldRev2 = Creator.GetCapabilitiesImplementedReviews();
      oldSolnEx.ClaimedCapabilityReview = new List<CapabilitiesImplementedReviews>(new[] { oldRev1, oldRev2 });

      var newSoln = Creator.GetSolution(status: status);
      var newSolnEx = Creator.GetSolutionEx(soln: newSoln);
      var newRev3 = Creator.GetCapabilitiesImplementedReviews();
      newSolnEx.ClaimedCapabilityReview = new List<CapabilitiesImplementedReviews>(new[] { oldRev1.Clone(), oldRev2.Clone(), newRev3 });


      var res = validator.MustBePendingToChangeClaimedCapabilityReview(oldSolnEx, newSolnEx);


      res.Should().BeTrue();
    }

    [Test]
    public void MustBePendingToChangeClaimedCapabilityReview_NotPending_AddReview_Fails(
      [ValueSource(nameof(SolutionStatusesNotPendingForReview))]SolutionStatus status)
    {
      var validator = GetValidator();

      var oldSolnEx = Creator.GetSolutionEx();
      var oldRev1 = Creator.GetCapabilitiesImplementedReviews();
      var oldRev2 = Creator.GetCapabilitiesImplementedReviews();
      oldSolnEx.ClaimedCapabilityReview = new List<CapabilitiesImplementedReviews>(new[] { oldRev1, oldRev2 });

      var newSoln = Creator.GetSolution(status: status);
      var newSolnEx = Creator.GetSolutionEx(soln: newSoln);
      var newRev3 = Creator.GetCapabilitiesImplementedReviews();
      newSolnEx.ClaimedCapabilityReview = new List<CapabilitiesImplementedReviews>(new[] { oldRev1.Clone(), oldRev2.Clone(), newRev3 });


      var res = validator.MustBePendingToChangeClaimedCapabilityReview(oldSolnEx, newSolnEx);


      res.Should().BeFalse();
    }

    [Test]
    public void MustBePendingToChangeClaimedCapabilityReview_DifferentReview_Fails(
      [ValueSource(typeof(Creator), nameof(Creator.SolutionStatuses))]SolutionStatus status)
    {
      var validator = GetValidator();

      var oldSolnEx = Creator.GetSolutionEx();
      var oldRev1 = Creator.GetCapabilitiesImplementedReviews();
      var oldRev2 = Creator.GetCapabilitiesImplementedReviews();
      oldSolnEx.ClaimedCapabilityReview = new List<CapabilitiesImplementedReviews>(new[] { oldRev1, oldRev2 });

      var newSoln = Creator.GetSolution(status: status);
      var newSolnEx = Creator.GetSolutionEx(soln: newSoln);
      var newRev1 = Creator.GetCapabilitiesImplementedReviews();
      var newRev2 = Creator.GetCapabilitiesImplementedReviews();
      newSolnEx.ClaimedCapabilityReview = new List<CapabilitiesImplementedReviews>(new[] { newRev1, newRev2 });


      var res = validator.MustBePendingToChangeClaimedCapabilityReview(oldSolnEx, newSolnEx);


      res.Should().BeFalse();
    }

    [Test]
    public void MustBePendingToChangeClaimedCapabilityReview_RemoveReview_Fails(
      [ValueSource(typeof(Creator), nameof(Creator.SolutionStatuses))]SolutionStatus status)
    {
      var validator = GetValidator();

      var oldSolnEx = Creator.GetSolutionEx();
      var oldRev1 = Creator.GetCapabilitiesImplementedReviews();
      var oldRev2 = Creator.GetCapabilitiesImplementedReviews();
      oldSolnEx.ClaimedCapabilityReview = new List<CapabilitiesImplementedReviews>(new[] { oldRev1, oldRev2 });

      var newSoln = Creator.GetSolution(status: status);
      var newSolnEx = Creator.GetSolutionEx(soln: newSoln);
      newSolnEx.ClaimedCapabilityReview = new List<CapabilitiesImplementedReviews>(new[] { oldRev1.Clone() });


      var res = validator.MustBePendingToChangeClaimedCapabilityReview(oldSolnEx, newSolnEx);


      res.Should().BeFalse();
    }
    #endregion

    #region ClaimedStandardReview
    [Test]
    public void MustBePendingToChangeClaimedStandardReview_SameReview_Succeeds(
      [ValueSource(typeof(Creator), nameof(Creator.SolutionStatuses))]SolutionStatus status)
    {
      var validator = GetValidator();

      var oldSolnEx = Creator.GetSolutionEx();
      var oldRev1 = Creator.GetStandardsApplicableReviews();
      var oldRev2 = Creator.GetStandardsApplicableReviews();
      oldSolnEx.ClaimedStandardReview = new List<StandardsApplicableReviews>(new[] { oldRev1, oldRev2 });

      var newSoln = Creator.GetSolution(status: status);
      var newSolnEx = Creator.GetSolutionEx(soln: newSoln);
      newSolnEx.ClaimedStandardReview = new List<StandardsApplicableReviews>(new[] { oldRev1.Clone(), oldRev2.Clone() });


      var res = validator.MustBePendingToChangeClaimedStandardReview(oldSolnEx, newSolnEx);


      res.Should().BeTrue();
    }

    [Test]
    public void MustBePendingToChangeClaimedStandardReview_Pending_AddReview_Succeeds(
      [ValueSource(nameof(SolutionStatusesPendingForReview))]SolutionStatus status)
    {
      var validator = GetValidator();

      var oldSolnEx = Creator.GetSolutionEx();
      var oldRev1 = Creator.GetStandardsApplicableReviews();
      var oldRev2 = Creator.GetStandardsApplicableReviews();
      oldSolnEx.ClaimedStandardReview = new List<StandardsApplicableReviews>(new[] { oldRev1, oldRev2 });

      var newSoln = Creator.GetSolution(status: status);
      var newSolnEx = Creator.GetSolutionEx(soln: newSoln);
      var newRev3 = Creator.GetStandardsApplicableReviews();
      newSolnEx.ClaimedStandardReview = new List<StandardsApplicableReviews>(new[] { oldRev1.Clone(), oldRev2.Clone(), newRev3 });


      var res = validator.MustBePendingToChangeClaimedStandardReview(oldSolnEx, newSolnEx);


      res.Should().BeTrue();
    }

    [Test]
    public void MustBePendingToChangeClaimedStandardReview_NotPending_AddReview_Fails(
      [ValueSource(nameof(SolutionStatusesNotPendingForReview))]SolutionStatus status)
    {
      var validator = GetValidator();

      var oldSolnEx = Creator.GetSolutionEx();
      var oldRev1 = Creator.GetStandardsApplicableReviews();
      var oldRev2 = Creator.GetStandardsApplicableReviews();
      oldSolnEx.ClaimedStandardReview = new List<StandardsApplicableReviews>(new[] { oldRev1, oldRev2 });

      var newSoln = Creator.GetSolution(status: status);
      var newSolnEx = Creator.GetSolutionEx(soln: newSoln);
      var newRev3 = Creator.GetStandardsApplicableReviews();
      newSolnEx.ClaimedStandardReview = new List<StandardsApplicableReviews>(new[] { oldRev1.Clone(), oldRev2.Clone(), newRev3 });


      var res = validator.MustBePendingToChangeClaimedStandardReview(oldSolnEx, newSolnEx);


      res.Should().BeFalse();
    }

    [Test]
    public void MustBePendingToChangeClaimedStandardReview_DifferentReview_Fails(
      [ValueSource(typeof(Creator), nameof(Creator.SolutionStatuses))]SolutionStatus status)
    {
      var validator = GetValidator();

      var oldSolnEx = Creator.GetSolutionEx();
      var oldRev1 = Creator.GetStandardsApplicableReviews();
      var oldRev2 = Creator.GetStandardsApplicableReviews();
      oldSolnEx.ClaimedStandardReview = new List<StandardsApplicableReviews>(new[] { oldRev1, oldRev2 });

      var newSoln = Creator.GetSolution(status: status);
      var newSolnEx = Creator.GetSolutionEx(soln: newSoln);
      var newRev1 = Creator.GetStandardsApplicableReviews();
      var newRev2 = Creator.GetStandardsApplicableReviews();
      newSolnEx.ClaimedStandardReview = new List<StandardsApplicableReviews>(new[] { newRev1, newRev2 });


      var res = validator.MustBePendingToChangeClaimedStandardReview(oldSolnEx, newSolnEx);


      res.Should().BeFalse();
    }

    [Test]
    public void MustBePendingToChangeClaimedStandardReview_RemoveReview_Fails(
      [ValueSource(typeof(Creator), nameof(Creator.SolutionStatuses))]SolutionStatus status)
    {
      var validator = GetValidator();

      var oldSolnEx = Creator.GetSolutionEx();
      var oldRev1 = Creator.GetStandardsApplicableReviews();
      var oldRev2 = Creator.GetStandardsApplicableReviews();
      oldSolnEx.ClaimedStandardReview = new List<StandardsApplicableReviews>(new[] { oldRev1, oldRev2 });

      var newSoln = Creator.GetSolution(status: status);
      var newSolnEx = Creator.GetSolutionEx(soln: newSoln);
      newSolnEx.ClaimedStandardReview = new List<StandardsApplicableReviews>(new[] { oldRev1.Clone() });


      var res = validator.MustBePendingToChangeClaimedStandardReview(oldSolnEx, newSolnEx);


      res.Should().BeFalse();
    }
    #endregion
    #endregion

    private SolutionsExValidator GetValidator()
    {
      return new SolutionsExValidator(_context.Object, _logger.Object, _datastore.Object, _solutionsValidator.Object);
    }
  }
}

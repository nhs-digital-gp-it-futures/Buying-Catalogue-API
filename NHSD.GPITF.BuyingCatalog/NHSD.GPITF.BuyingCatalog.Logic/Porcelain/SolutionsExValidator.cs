using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NHSD.GPITF.BuyingCatalog.Interfaces;
using NHSD.GPITF.BuyingCatalog.Interfaces.Porcelain;
using NHSD.GPITF.BuyingCatalog.Models;
using NHSD.GPITF.BuyingCatalog.Models.Porcelain;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NHSD.GPITF.BuyingCatalog.Logic.Porcelain
{
  public sealed class SolutionsExValidator : ValidatorBase<SolutionEx>, ISolutionsExValidator
  {
    private readonly ISolutionsExDatastore _datastore;
    private readonly ISolutionsValidator _solutionsValidator;

    public SolutionsExValidator(
      IHttpContextAccessor context,
      ILogger<SolutionsExValidator> logger,
      ISolutionsExDatastore datastore,
      ISolutionsValidator solutionsValidator) :
      base(context, logger)
    {
      _datastore = datastore;
      _solutionsValidator = solutionsValidator;

      RuleSet(nameof(ISolutionsExLogic.Update), () =>
      {
        // use Solution validator
        MustBeValidSolution();

        // internal consistency checks
        ClaimedCapabilityMustBelongToSolution();
        ClaimedCapabilityEvidenceMustBelongToClaim();

        ClaimedStandardMustBelongToSolution();
        ClaimedStandardEvidenceMustBelongToClaim();

        TechnicalContactMustBelongToSolution();

        // all previous versions in solution
        ClaimedCapabilityEvidencePreviousVersionMustBelongToSolution();
        ClaimedStandardEvidencePreviousVersionMustBelongToSolution();

        // One Rule to rule them all,
        // One Rule to find them,
        // One Rule to bring them all,
        // and in the darkness bind them
        CheckUpdateAllowed();
      });
    }

    public void MustBeValidSolution()
    {
      RuleFor(x => x.Solution)
        .Must(soln =>
        {
          _solutionsValidator.ValidateAndThrowEx(soln, ruleSet: nameof(ISolutionsLogic.Update));
          return true;
        });
    }

    public void ClaimedCapabilityMustBelongToSolution()
    {
      RuleFor(x => x)
        .Must(soln =>
        {
          return soln.ClaimedCapability.All(cc => cc.SolutionId == soln.Solution.Id);
        })
        .WithMessage("ClaimedCapability must belong to solution");
    }

    public void ClaimedStandardMustBelongToSolution()
    {
      RuleFor(x => x)
        .Must(soln =>
        {
          return soln.ClaimedStandard.All(cs => cs.SolutionId == soln.Solution.Id);
        })
        .WithMessage("ClaimedStandard must belong to solution");
    }

    public void ClaimedCapabilityEvidenceMustBelongToClaim()
    {
      RuleFor(x => x)
        .Must(soln =>
        {
          var claimIds = soln.ClaimedCapability.Select(cc => cc.Id);
          return soln.ClaimedCapabilityEvidence.All(cce => claimIds.Contains(cce.ClaimId));
        })
        .WithMessage("ClaimedCapabilityEvidence must belong to claim");
    }

    public void ClaimedStandardEvidenceMustBelongToClaim()
    {
      RuleFor(x => x)
        .Must(soln =>
        {
          var claimIds = soln.ClaimedStandard.Select(cs => cs.Id);
          return soln.ClaimedStandardEvidence.All(cse => claimIds.Contains(cse.ClaimId));
        })
        .WithMessage("ClaimedStandardEvidence must belong to claim");
    }

    public void TechnicalContactMustBelongToSolution()
    {
      RuleFor(x => x)
        .Must(soln =>
        {
          return soln.TechnicalContact.All(tc => tc.SolutionId == soln.Solution.Id);
        })
        .WithMessage("TechnicalContact must belong to solution");
    }

    public void ClaimedCapabilityEvidencePreviousVersionMustBelongToSolution()
    {
      RuleFor(x => x)
        .Must(soln =>
        {
          var evidenceIds = soln.ClaimedCapabilityEvidence.Select(cce => cce.Id);
          var evidencePrevIds = soln.ClaimedCapabilityEvidence.Select(cce => cce.PreviousId).Where(id => id != null);
          return evidencePrevIds.All(prevId => evidenceIds.Contains(prevId));
        })
        .WithMessage("ClaimedCapabilityEvidence previous version must belong to solution");
    }

    public void ClaimedStandardEvidencePreviousVersionMustBelongToSolution()
    {
      RuleFor(x => x)
        .Must(soln =>
        {
          var evidenceIds = soln.ClaimedStandardEvidence.Select(cce => cce.Id);
          var evidencePrevIds = soln.ClaimedStandardEvidence.Select(cce => cce.PreviousId).Where(id => id != null);
          return evidencePrevIds.All(prevId => evidenceIds.Contains(prevId));
        })
        .WithMessage("ClaimedStandardEvidence previous version must belong to solution");
    }

    public void CheckUpdateAllowed()
    {
      RuleFor(x => x)
        .Must(newSolnEx =>
        {
          var oldSolnEx = _datastore.BySolution(newSolnEx.Solution.Id);
          return MustBePendingToChangeClaimedCapability(oldSolnEx, newSolnEx);
        })
        .WithMessage("Must Be Pending To Change Claimed Capability");

      RuleFor(x => x)
        .Must(newSolnEx =>
        {
          var oldSolnEx = _datastore.BySolution(newSolnEx.Solution.Id);
          return MustBePendingToChangeClaimedStandard(oldSolnEx, newSolnEx);
        })
        .WithMessage("Must Be Pending To Change Claimed Standard");

      RuleFor(x => x)
        .Must(newSolnEx =>
        {
          var oldSolnEx = _datastore.BySolution(newSolnEx.Solution.Id);
          return MustBePendingToChangeClaimedCapabilityEvidence(oldSolnEx, newSolnEx);
        })
        .WithMessage("Must Be Pending To Change Claimed Capability Evidence");

      RuleFor(x => x)
        .Must(newSolnEx =>
        {
          var oldSolnEx = _datastore.BySolution(newSolnEx.Solution.Id);
          return MustBePendingToChangeClaimedStandardEvidence(oldSolnEx, newSolnEx);
        })
        .WithMessage("Must Be Pending To Change Claimed Standard Evidence");
    }

    private static bool MustBePendingToChangeClaim<T>(
      SolutionStatus newSolnStatus,
      IEnumerable<T> oldItems,
      IEnumerable<T> newItems,
      IEqualityComparer<T> comparer,
      Action onError
      ) where T : IHasId
    {
      var newNotOld = newItems.Except(oldItems, comparer).ToList();
      var oldNotNew = oldItems.Except(newItems, comparer).ToList();
      var same = !newNotOld.Any() && !oldNotNew.Any();

      if (same)
      {
        // no add/remove
        return true;
      }

      if ((oldNotNew.Any() || newNotOld.Any()) &&
        !IsPendingForClaims(newSolnStatus))
      {
        // Can only add/remove Claim while pending
        onError();
        return false;
      }

      return true;
    }

    // can only add/remove ClaimedCapability while pending
    public bool MustBePendingToChangeClaimedCapability(SolutionEx oldSolnEx, SolutionEx newSolnEx)
    {
      var same = MustBePendingToChangeClaim(
        newSolnEx.Solution.Status,
        oldSolnEx.ClaimedCapability,
        newSolnEx.ClaimedCapability,
        new CapabilitiesImplementedComparer(),
        () =>
        {
          // Can only add/remove ClaimedCapability while pending
          var msg = new { ErrorMessage = nameof(MustBePendingToChangeClaimedCapability), ExistingValue = oldSolnEx };
          _logger.LogError(JsonConvert.SerializeObject(msg));
        });

      return same;
    }

    // can only add/remove ClaimedStandard while pending
    public bool MustBePendingToChangeClaimedStandard(SolutionEx oldSolnEx, SolutionEx newSolnEx)
    {
      var same = MustBePendingToChangeClaim(
        newSolnEx.Solution.Status,
        oldSolnEx.ClaimedStandard,
        newSolnEx.ClaimedStandard,
        new StandardsApplicableComparer(),
        () =>
        {
          // Can only add/remove ClaimedStandard while pending
          var msg = new { ErrorMessage = nameof(MustBePendingToChangeClaimedStandard), ExistingValue = oldSolnEx };
          _logger.LogError(JsonConvert.SerializeObject(msg));
        });

      return same;
    }

    private static bool MustBePendingToChangeEvidence<T>(
      SolutionStatus newSolnStatus,
      IEnumerable<T> oldItems,
      IEnumerable<T> newItems,
      IEqualityComparer<T> comparer,
      Action onError
      ) where T : IHasId
    {
      var newNotOld = newItems.Except(oldItems, comparer).ToList();
      var oldNotNew = oldItems.Except(newItems, comparer).ToList();

      if (newNotOld.Any() &&
        newItems.Count() > oldItems.Count() &&
        IsPendingForEvidence(newSolnStatus))
      {
        // added
        return true;
      }

      if (oldNotNew.Any() &&
        oldItems.Count() > newItems.Count() &&
        IsPendingForEvidence(newSolnStatus))
      {
        // removed
        return true;
      }

      var same = (!newNotOld.Any() && !oldNotNew.Any()) ||
        IsPendingForEvidence(newSolnStatus);
      if (!same)
      {
        onError();
      }

      return same;
    }

    // cannot change/remove ClaimedCapabilityEvidence but can add while pending
    public bool MustBePendingToChangeClaimedCapabilityEvidence(SolutionEx oldSolnEx, SolutionEx newSolnEx)
    {
      return MustBePendingToChangeEvidence(
        newSolnEx.Solution.Status,
        oldSolnEx.ClaimedCapabilityEvidence,
        newSolnEx.ClaimedCapabilityEvidence,
        new CapabilitiesImplementedEvidenceComparer(),
        () =>
        {
          var msg = new { ErrorMessage = nameof(MustBePendingToChangeClaimedCapabilityEvidence), ExistingValue = oldSolnEx };
          _logger.LogError(JsonConvert.SerializeObject(msg));
        });
    }

    // cannot change/remove ClaimedStandardEvidence but can add while pending
    public bool MustBePendingToChangeClaimedStandardEvidence(SolutionEx oldSolnEx, SolutionEx newSolnEx)
    {
      return MustBePendingToChangeEvidence(
        newSolnEx.Solution.Status,
        oldSolnEx.ClaimedStandardEvidence,
        newSolnEx.ClaimedStandardEvidence,
        new StandardsApplicableEvidenceComparer(),
        () =>
        {
          var msg = new { ErrorMessage = nameof(MustBePendingToChangeClaimedStandardEvidence), ExistingValue = oldSolnEx };
          _logger.LogError(JsonConvert.SerializeObject(msg));
        });
    }


    private static bool MustBePendingToChangeReview<T>(
      SolutionStatus newSolnStatus,
      IEnumerable<T> oldItems,
      IEnumerable<T> newItems,
      IEqualityComparer<T> comparer,
      Action onError
      ) where T : IHasId
    {
      var newNotOld = newItems.Except(oldItems, comparer).ToList();
      var oldNotNew = oldItems.Except(newItems, comparer).ToList();

      if (newNotOld.Any() &&
        newNotOld.Count > oldNotNew.Count &&
        IsPendingForReview(newSolnStatus))
      {
        // added
        return true;
      }

      var same = !newNotOld.Any() && !oldNotNew.Any();
      if (!same)
      {
        onError();
      }

      return same;
    }

    // check every ClaimedCapability
    // check every ClaimedStandard
    // check every ClaimedCapabilityEvidence
    // check every ClaimedStandardEvidence
    // check every ClaimedCapabilityReview
    // check every ClaimedStandardReview

    private static bool IsPendingForClaims(SolutionStatus status)
    {
      return status == SolutionStatus.Draft ||
        status == SolutionStatus.Registered ||
        status == SolutionStatus.CapabilitiesAssessment ||
        status == SolutionStatus.StandardsCompliance;
    }

    private static bool IsPendingForEvidence(SolutionStatus status)
    {
      return
        status == SolutionStatus.Registered ||
        status == SolutionStatus.CapabilitiesAssessment ||
        status == SolutionStatus.StandardsCompliance;
    }

    private static bool IsPendingForReview(SolutionStatus status)
    {
      return status == SolutionStatus.CapabilitiesAssessment ||
        status == SolutionStatus.StandardsCompliance;
    }
  }
}

﻿using NHSD.GPITF.BuyingCatalog.Logic;
using NHSD.GPITF.BuyingCatalog.Models;
using NHSD.GPITF.BuyingCatalog.Models.Porcelain;
using System;
using System.Collections.Generic;

namespace NHSD.GPITF.BuyingCatalog.Tests
{
  public static class Creator
  {
    public static Frameworks GetFramework(
      string id = null)
    {
      var retval = new Frameworks
      {
        Id = id ?? Guid.NewGuid().ToString()
      };
      Verifier.Verify(retval);
      return retval;
    }

    public static Contacts GetContact(
      string id = null,
      string orgId = null,
      string emailAddress1 = null)
    {
      var retval = new Contacts
      {
        Id = id ?? Guid.NewGuid().ToString(),
        OrganisationId = orgId ?? Guid.NewGuid().ToString(),
        EmailAddress1 = emailAddress1 ?? "jon.dough@tpp.com"
      };
      Verifier.Verify(retval);
      return retval;
    }

    public static Organisations GetOrganisation(
      string id = "NHS Digital",
      string primaryRoleId = PrimaryRole.GovernmentDepartment)
    {
      var retval = new Organisations
      {
        Id = id,
        Name = id,
        PrimaryRoleId = primaryRoleId
      };
      Verifier.Verify(retval);
      return retval;
    }

    public static Solutions GetSolution(
      string id = null,
      string previousId = null,
      string orgId = null,
      SolutionStatus status = SolutionStatus.Draft,
      string createdById = null,
      DateTime? createdOn = null,
      string modifiedById = null,
      DateTime? modifiedOn = null)
    {
      var retval = new Solutions
      {
        Id = id ?? Guid.NewGuid().ToString(),
        PreviousId = previousId,
        OrganisationId = orgId ?? Guid.NewGuid().ToString(),
        Status = status,
        CreatedById = createdById ?? Guid.NewGuid().ToString(),
        CreatedOn = createdOn ?? DateTime.Now,
        ModifiedById = modifiedById ?? Guid.NewGuid().ToString(),
        ModifiedOn = modifiedOn ?? DateTime.Now
      };
      Verifier.Verify(retval);
      return retval;
    }

    public static SolutionEx GetSolutionEx(
      Solutions soln = null,

      List<CapabilitiesImplemented> claimedCap = null,

      List<StandardsApplicable> claimedStd = null,

      List<TechnicalContacts> techCont = null
      )
    {
      soln = soln ?? GetSolution();

      claimedCap = claimedCap ?? new List<CapabilitiesImplemented>
      {
        GetCapabilitiesImplemented(solnId: soln.Id)
      };

      claimedStd = claimedStd ?? new List<StandardsApplicable>
      {
        GetStandardsApplicable(solnId: soln.Id)
      };

      techCont = techCont ?? new List<TechnicalContacts>
      {
        GetTechnicalContact(solutionId: soln.Id)
      };

      var solnEx = new SolutionEx
      {
        Solution = soln,

        ClaimedCapability = claimedCap,

        ClaimedStandard = claimedStd,

        TechnicalContact = techCont
      };

      Verifier.Verify(solnEx);
      return solnEx;
    }

    public static TechnicalContacts GetTechnicalContact(
      string id = null,
      string solutionId = null,
      string contactType = "Technical Contact",
      string emailAddress = "jon.dough@tpp.com"
      )
    {
      var retval = new TechnicalContacts
      {
        Id = id ?? Guid.NewGuid().ToString(),
        SolutionId = solutionId ?? Guid.NewGuid().ToString(),
        ContactType = contactType,
        EmailAddress = emailAddress
      };
      Verifier.Verify(retval);
      return retval;
    }

    public static Capabilities GetCapability(
      string id = null,
      string name = null,
      string description = null)
    {
      var retval = new Capabilities
      {
        Id = id ?? Guid.NewGuid().ToString(),
        Name = name ?? string.Empty,
        Description = description ?? string.Empty
      };
      Verifier.Verify(retval);
      return retval;
    }

    public static ClaimsBase GetClaimsBase(
      string id = null,
      string solnId = null,
      string ownerId = null,
      DateTime? originalDate = null)
    {
      var retval = new DummyClaimsBase
      {
        Id = id ?? Guid.NewGuid().ToString(),
        SolutionId = solnId ?? Guid.NewGuid().ToString(),
        OwnerId = ownerId ?? Guid.NewGuid().ToString(),
        OriginalDate = originalDate ?? DateTime.UtcNow
      };
      Verifier.Verify(retval);
      return retval;
    }

    public static CapabilitiesImplemented GetCapabilitiesImplemented(
      string id = null,
      string solnId = null,
      string claimId = null,
      string ownerId = null,
      CapabilitiesImplementedStatus status = CapabilitiesImplementedStatus.Draft,
      DateTime? originalDate = null)
    {
      var retval = new CapabilitiesImplemented
      {
        Id = id ?? Guid.NewGuid().ToString(),
        SolutionId = solnId ?? Guid.NewGuid().ToString(),
        CapabilityId = claimId ?? Guid.NewGuid().ToString(),
        OwnerId = ownerId ?? Guid.NewGuid().ToString(),
        Status = status,
        OriginalDate = originalDate ?? DateTime.UtcNow
      };
      Verifier.Verify(retval);
      return retval;
    }

    public static StandardsApplicable GetStandardsApplicable(
      string id = null,
      string solnId = null,
      string claimId = null,
      string ownerId = null,
      StandardsApplicableStatus status = StandardsApplicableStatus.Draft,
      DateTime? originalDate = null,
      DateTime? submittedOn = null,
      DateTime? assignedOn = null)
    {
      var retval = new StandardsApplicable
      {
        Id = id ?? Guid.NewGuid().ToString(),
        SolutionId = solnId ?? Guid.NewGuid().ToString(),
        StandardId = claimId ?? Guid.NewGuid().ToString(),
        OwnerId = ownerId ?? Guid.NewGuid().ToString(),
        Status = status,
        OriginalDate = originalDate ?? DateTime.UtcNow,
        SubmittedOn = submittedOn ?? DateTime.UtcNow,
        AssignedOn = assignedOn ?? DateTime.UtcNow
      };
      Verifier.Verify(retval);
      return retval;
    }

    public static IEnumerable<SolutionStatus> SolutionStatuses()
    {
      return (SolutionStatus[])Enum.GetValues(typeof(SolutionStatus));
    }
  }
}

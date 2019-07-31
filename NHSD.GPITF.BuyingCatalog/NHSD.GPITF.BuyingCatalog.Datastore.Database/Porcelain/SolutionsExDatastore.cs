using Dapper.Contrib.Extensions;
using Microsoft.Extensions.Logging;
using NHSD.GPITF.BuyingCatalog.Datastore.Database.Interfaces;
using NHSD.GPITF.BuyingCatalog.Interfaces;
using NHSD.GPITF.BuyingCatalog.Interfaces.Porcelain;
using NHSD.GPITF.BuyingCatalog.Models.Porcelain;
using System.Collections.Generic;
using System.Linq;

namespace NHSD.GPITF.BuyingCatalog.Datastore.Database.Porcelain
{
  public sealed class SolutionsExDatastore : DatastoreBase<SolutionEx>, ISolutionsExDatastore
  {
    private readonly ISolutionsDatastore _solutionDatastore;
    private readonly ITechnicalContactsDatastore _technicalContactDatastore;

    private readonly ICapabilitiesImplementedDatastore _claimedCapabilityDatastore;

    private readonly IStandardsApplicableDatastore _claimedStandardDatastore;

    public SolutionsExDatastore(
      IDbConnectionFactory dbConnectionFactory,
      ILogger<SolutionsExDatastore> logger,
      ISyncPolicyFactory policy,
      ISolutionsDatastore solutionDatastore,
      ITechnicalContactsDatastore technicalContactDatastore,

      ICapabilitiesImplementedDatastore claimedCapabilityDatastore,

      IStandardsApplicableDatastore claimedStandardDatastore
      ) :
      base(dbConnectionFactory, logger, policy)
    {
      _solutionDatastore = solutionDatastore;
      _technicalContactDatastore = technicalContactDatastore;

      _claimedCapabilityDatastore = claimedCapabilityDatastore;

      _claimedStandardDatastore = claimedStandardDatastore;
    }

    public SolutionEx BySolution(string solutionId)
    {
      return GetInternal(() =>
      {
        var retval = new SolutionEx
        {
          Solution = _solutionDatastore.ById(solutionId),
          TechnicalContact = _technicalContactDatastore.BySolution(solutionId).ToList(),
          ClaimedCapability = _claimedCapabilityDatastore.BySolution(solutionId).ToList(),
          ClaimedStandard = _claimedStandardDatastore.BySolution(solutionId).ToList()
        };

        return retval;
      });
    }

    public void Update(SolutionEx solnEx)
    {
      GetInternal(() =>
      {
        using (var trans = _dbConnection.BeginTransaction())
        {
          // update Solution
          _dbConnection.Update(solnEx.Solution, trans);

          #region ClaimedCapability
          // delete ClaimedCapabilities which will cascade delete Evidence + Reviews
          _claimedCapabilityDatastore
            .BySolution(solnEx.Solution.Id)
            .ToList()
            .ForEach(cc => _dbConnection.Delete(cc, trans));

          // re-insert ClaimedCapabilities + Evidence + Reviews
          solnEx.ClaimedCapability.ForEach(cc => _dbConnection.Insert(cc, trans));
          #endregion

          #region ClaimedStandard
          // delete ClaimedStandards which will cascade delete Evidence + Reviews
          _claimedStandardDatastore
            .BySolution(solnEx.Solution.Id)
            .ToList()
            .ForEach(cs => _dbConnection.Delete(cs, trans));

          // re-insert ClaimedStandards + Evidence + Reviews
          solnEx.ClaimedStandard.ForEach(cs => _dbConnection.Insert(cs, trans));
          #endregion

          #region TechnicalContacts
          // delete all TechnicalContact & re-insert
          _technicalContactDatastore
            .BySolution(solnEx.Solution.Id).ToList()
            .ForEach(tc => _dbConnection.Delete(tc, trans));
          solnEx.TechnicalContact.ForEach(tc => _dbConnection.Insert(tc, trans));
          #endregion

          trans.Commit();
        }

        return 0;
      });
    }

    public IEnumerable<SolutionEx> ByOrganisation(string organisationId)
    {
      return GetInternal(() =>
      {
        var solns = _solutionDatastore.ByOrganisation(organisationId);
        var retval = solns.Select(soln => BySolution(soln.Id));

        return retval;
      });
    }
  }
}

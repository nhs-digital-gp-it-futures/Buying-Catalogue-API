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
    private readonly IOrganisationsDatastore _organisationsDatastore;

    public SolutionsExDatastore(
      IDbConnectionFactory dbConnectionFactory,
      ILogger<SolutionsExDatastore> logger,
      ISyncPolicyFactory policy,
      ISolutionsDatastore solutionDatastore,
      ITechnicalContactsDatastore technicalContactDatastore,
      ICapabilitiesImplementedDatastore claimedCapabilityDatastore,
      IStandardsApplicableDatastore claimedStandardDatastore,
      IOrganisationsDatastore organisationsDatastore
      ) :
      base(dbConnectionFactory, logger, policy)
    {
      _solutionDatastore = solutionDatastore;
      _technicalContactDatastore = technicalContactDatastore;
      _claimedCapabilityDatastore = claimedCapabilityDatastore;
      _claimedStandardDatastore = claimedStandardDatastore;
      _organisationsDatastore = organisationsDatastore;
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
          ClaimedStandard = _claimedStandardDatastore.BySolution(solutionId).ToList(),
          Organisation = _organisationsDatastore.ById(_solutionDatastore.ById(solutionId)?.OrganisationId)
        };

        return retval;
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

using Dapper;
using Microsoft.Extensions.Logging;
using NHSD.GPITF.BuyingCatalog.Datastore.Database.Interfaces;
using NHSD.GPITF.BuyingCatalog.Interfaces;
using NHSD.GPITF.BuyingCatalog.Interfaces.Porcelain;
using NHSD.GPITF.BuyingCatalog.Models;
using NHSD.GPITF.BuyingCatalog.Models.Porcelain;
using System.Collections.Generic;
using System.Linq;

namespace NHSD.GPITF.BuyingCatalog.Datastore.Database.Porcelain
{
  public sealed class SearchDatastore : DatastoreBase<Solutions>, ISearchDatastore
  {
    private readonly IFrameworksDatastore _frameworkDatastore;
    private readonly ISolutionsDatastore _solutionDatastore;
    private readonly ICapabilitiesImplementedDatastore _claimedCapabilityDatastore;
    private readonly ISolutionsExDatastore _solutionsExDatastore;

    public SearchDatastore(
      IDbConnectionFactory dbConnectionFactory, 
      ILogger<SearchDatastore> logger, 
      ISyncPolicyFactory policy,
      IFrameworksDatastore frameworkDatastore,
      ISolutionsDatastore solutionDatastore,
      ICapabilitiesImplementedDatastore claimedCapabilityDatastore,
      ISolutionsExDatastore solutionsExDatastore) :
      base(dbConnectionFactory, logger, policy)
    {
      _frameworkDatastore = frameworkDatastore;
      _solutionDatastore = solutionDatastore;
      _claimedCapabilityDatastore = claimedCapabilityDatastore;
      _solutionsExDatastore = solutionsExDatastore;
    }

    public IEnumerable<SolutionEx> ByCapabilities(IEnumerable<string> capIds)
    {
      var capIdsQuoted = capIds.Select(x => $"'{x}'");
      var capsSet = string.Join(",", capIdsQuoted);

      _logger.LogInformation($"{capsSet}");

      return GetInternal(() =>
      {
        // get all Frameworks
        var allFrameworks = _frameworkDatastore.GetAll();

        // get all Solutions via frameworks
        var allSolns = allFrameworks
          .SelectMany(fw => _solutionDatastore.ByFramework(fw.Id));

        // get all unique Solutions with at least all specified Capability
        var allSolnsCapsIds = allSolns
          .Where(soln => _claimedCapabilityDatastore
            .BySolution(soln.Id)
            .Select(cc => cc.CapabilityId)
            .Intersect(capIds)
            .Count() >= capIds.Count())
          .Select(soln => soln.Id);

        var retval = allSolnsCapsIds.Select(solnId => _solutionsExDatastore.BySolution(solnId));

        return retval;
      });
    }
  }
}

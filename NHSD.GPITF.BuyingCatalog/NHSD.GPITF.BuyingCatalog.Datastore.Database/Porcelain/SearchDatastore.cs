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
    private readonly ISolutionsExDatastore _solutionsExDatastore;

    public SearchDatastore(
      IDbConnectionFactory dbConnectionFactory, 
      ILogger<SearchDatastore> logger, 
      ISyncPolicyFactory policy,
      ISolutionsExDatastore solutionsExDatastore) :
      base(dbConnectionFactory, logger, policy)
    {
      _solutionsExDatastore = solutionsExDatastore;
    }

    public IEnumerable<SolutionEx> ByCapabilities(IEnumerable<string> capIds)
    {
      var capIdsQuoted = capIds.Select(x => $"'{x}'");
      var capsSet = string.Join(",", capIdsQuoted);

      _logger.LogInformation($"{capsSet}");

      return GetInternal(() =>
      {
        var sql =
$@"
SELECT soln.Id FROM Solutions soln
join CapabilitiesImplemented ci on ci.SolutionId = soln.Id
join Capabilities cap on cap.Id = ci.CapabilityId
where cap.Id in ({capsSet})
group by soln.Id
having count(soln.Id) = {capIds.Count()}
";
        var solnIds = _dbConnection.Query<string>(sql);
        var retval = solnIds.Select(solnId => _solutionsExDatastore.BySolution(solnId));

        return retval;
      });
    }
  }
}

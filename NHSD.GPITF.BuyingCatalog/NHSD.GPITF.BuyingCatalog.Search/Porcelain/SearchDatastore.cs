using Microsoft.Extensions.Logging;
using NHSD.GPITF.BuyingCatalog.Interfaces;
using NHSD.GPITF.BuyingCatalog.Interfaces.Porcelain;
using NHSD.GPITF.BuyingCatalog.Models.Porcelain;
using Polly;
using System.Collections.Generic;
using System.Linq;

namespace NHSD.GPITF.BuyingCatalog.Search.Porcelain
{
  public sealed class SearchDatastore : ISearchDatastore
  {
    private readonly ILogger<SearchDatastore> _logger;
    private readonly ISyncPolicy _policy;
    private readonly IFrameworksDatastore _frameworkDatastore;
    private readonly ISolutionsDatastore _solutionDatastore;
    private readonly ICapabilitiesDatastore _capabilityDatastore;
    private readonly ICapabilitiesImplementedDatastore _claimedCapabilityDatastore;
    private readonly ISolutionsExDatastore _solutionExDatastore;

    public SearchDatastore(
      ILogger<SearchDatastore> logger,
      ISyncPolicyFactory policy,
      IFrameworksDatastore frameworkDatastore,
      ISolutionsDatastore solutionDatastore,
      ICapabilitiesDatastore capabilityDatastore,
      ICapabilitiesImplementedDatastore claimedCapabilityDatastore,
      ISolutionsExDatastore solutionExDatastore)
    {
      _logger = logger;
      _policy = policy.Build(_logger);
      _frameworkDatastore = frameworkDatastore;
      _solutionDatastore = solutionDatastore;
      _capabilityDatastore = capabilityDatastore;
      _claimedCapabilityDatastore = claimedCapabilityDatastore;
      _solutionExDatastore = solutionExDatastore;
    }

    public IEnumerable<SearchResult> ByCapabilities(IEnumerable<string> capIds)
    {
      _logger.LogInformation($"{capIds}");

      return _policy.Execute(() =>
      {
        return Enumerable.Empty<SearchResult>();
      });
    }
  }
}

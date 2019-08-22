using Microsoft.AspNetCore.Http;
using NHSD.GPITF.BuyingCatalog.Interfaces.Porcelain;
using NHSD.GPITF.BuyingCatalog.Models.Porcelain;
using System.Collections.Generic;

namespace NHSD.GPITF.BuyingCatalog.Logic.Porcelain
{
  public sealed class SearchLogic : LogicBase, ISearchLogic
  {
    private readonly ISearchDatastore _datastore;
    private readonly ISolutionsExFilter _solutionExFilter;

    public SearchLogic(
      IHttpContextAccessor context,
      ISearchDatastore datastore,
      ISolutionsExFilter solutionExFilter) :
      base(context)
    {
      _datastore = datastore;
      _solutionExFilter = solutionExFilter;
    }

    public IEnumerable<SolutionEx> ByCapabilities(IEnumerable<string> capIds)
    {
      var searchResults = _datastore.ByCapabilities(capIds);
      return _solutionExFilter.Filter(searchResults);
    }
  }
}

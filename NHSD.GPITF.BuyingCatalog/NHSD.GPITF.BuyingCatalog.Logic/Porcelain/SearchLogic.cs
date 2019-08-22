using Microsoft.AspNetCore.Http;
using NHSD.GPITF.BuyingCatalog.Interfaces.Porcelain;
using NHSD.GPITF.BuyingCatalog.Models.Porcelain;
using System.Collections.Generic;
using System.Linq;

namespace NHSD.GPITF.BuyingCatalog.Logic.Porcelain
{
  public sealed class SearchLogic : LogicBase, ISearchLogic
  {
    private readonly ISearchDatastore _datastore;
    private readonly ISolutionsFilter _solutionFilter;

    public SearchLogic(
      IHttpContextAccessor context,
      ISearchDatastore datastore,
      ISolutionsFilter solutionFilter) :
      base(context)
    {
      _datastore = datastore;
      _solutionFilter = solutionFilter;
    }

    public IEnumerable<SearchResult> ByCapabilities(IEnumerable<string> capIds)
    {
      var searchResults = _datastore.ByCapabilities(capIds);
      return searchResults.Where(sr => _solutionFilter.Filter(new[] { sr.SolutionEx.Solution }).Any());
    }
  }
}

using NHSD.GPITF.BuyingCatalog.Interfaces;
using NHSD.GPITF.BuyingCatalog.Models;
using System.Linq;

namespace NHSD.GPITF.BuyingCatalog.Logic
{
  public sealed class TechnicalContactsFilter : FilterBase<TechnicalContacts>, ITechnicalContactsFilter
  {
    private readonly ISolutionsDatastore _solutionDatastore;
    private readonly ISolutionsFilter _solutionsFilter;

    public TechnicalContactsFilter(
      ISolutionsDatastore solutionDatastore,
      ISolutionsFilter solutionsFilter) :
      base()
    {
      _solutionDatastore = solutionDatastore;
      _solutionsFilter = solutionsFilter;
    }

    public override TechnicalContacts Filter(TechnicalContacts input)
    {
      // None:  only approved solutions
      var noneSoln = _solutionsFilter.Filter(new[] { _solutionDatastore.ById(input.SolutionId) }).SingleOrDefault();
      if (noneSoln == null)
      {
        return null;
      }

      return input;
    }
  }
}

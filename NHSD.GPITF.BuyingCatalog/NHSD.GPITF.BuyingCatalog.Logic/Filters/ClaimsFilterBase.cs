using NHSD.GPITF.BuyingCatalog.Interfaces;
using NHSD.GPITF.BuyingCatalog.Models;
using System.Linq;

namespace NHSD.GPITF.BuyingCatalog.Logic
{
  public abstract class ClaimsFilterBase<T> : FilterBase<T>, IClaimsFilter<T> where T : ClaimsBase
  {
    private readonly ISolutionsDatastore _solutionDatastore;
    private readonly ISolutionsFilter _solutionsFilter;

    protected ClaimsFilterBase(
      ISolutionsDatastore solutionDatastore,
      ISolutionsFilter solutionsFilter) :
      base()
    {
      _solutionDatastore = solutionDatastore;
      _solutionsFilter = solutionsFilter;
    }

    protected virtual T FilterSpecific(T input)
    {
      return input;
    }

    public override T Filter(T input)
    {
      input = FilterForNone(input);

      return FilterSpecific(input);
    }

    public T FilterForNone(T input)
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

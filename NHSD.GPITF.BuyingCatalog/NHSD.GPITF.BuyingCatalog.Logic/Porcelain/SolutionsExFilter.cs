using NHSD.GPITF.BuyingCatalog.Models.Porcelain;

namespace NHSD.GPITF.BuyingCatalog.Logic.Porcelain
{
  public sealed class SolutionsExFilter : FilterBase<SolutionEx>, ISolutionsExFilter
  {
    private readonly ISolutionsFilter _solutionsFilter;

    public SolutionsExFilter(
      ISolutionsFilter solutionsFilter) :
      base()
    {
      _solutionsFilter = solutionsFilter;
    }

    public override SolutionEx Filter(SolutionEx input)
    {
      return _solutionsFilter.Filter(new[] { input?.Solution }) != null ? input : null;
    }
  }
}

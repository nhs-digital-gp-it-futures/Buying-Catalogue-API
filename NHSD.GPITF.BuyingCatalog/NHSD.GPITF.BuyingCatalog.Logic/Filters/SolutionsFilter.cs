using NHSD.GPITF.BuyingCatalog.Models;

namespace NHSD.GPITF.BuyingCatalog.Logic
{
  public sealed class SolutionsFilter : FilterBase<Solutions>, ISolutionsFilter
  {
    public override Solutions Filter(Solutions input)
    {
      // None: only approved Solutions
      return (input.Status == SolutionStatus.Approved) ? input : null;
    }
  }
}

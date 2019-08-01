using NHSD.GPITF.BuyingCatalog.Interfaces;
using NHSD.GPITF.BuyingCatalog.Logic;
using NHSD.GPITF.BuyingCatalog.Models;

namespace NHSD.GPITF.BuyingCatalog.Tests
{
  public sealed class DummyClaimsFilterBase : ClaimsFilterBase<ClaimsBase>
  {
    public DummyClaimsFilterBase(
      ISolutionsDatastore solutionDatastore,
      ISolutionsFilter solutionsFilter) :
      base(solutionDatastore, solutionsFilter)
    {
    }
  }
}

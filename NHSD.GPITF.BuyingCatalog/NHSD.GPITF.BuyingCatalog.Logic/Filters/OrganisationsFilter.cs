using NHSD.GPITF.BuyingCatalog.Models;

namespace NHSD.GPITF.BuyingCatalog.Logic
{
  public sealed class OrganisationsFilter : FilterBase<Organisations>, IOrganisationsFilter
  {
    public override Organisations Filter(Organisations input)
    {
      return input;
    }
  }
}

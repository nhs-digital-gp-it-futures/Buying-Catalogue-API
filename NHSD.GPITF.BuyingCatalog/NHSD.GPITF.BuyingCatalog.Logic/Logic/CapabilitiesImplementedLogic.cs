using Microsoft.AspNetCore.Http;
using NHSD.GPITF.BuyingCatalog.Interfaces;
using NHSD.GPITF.BuyingCatalog.Models;

namespace NHSD.GPITF.BuyingCatalog.Logic
{
  public sealed class CapabilitiesImplementedLogic : ClaimsLogicBase<CapabilitiesImplemented>, ICapabilitiesImplementedLogic
  {
    public CapabilitiesImplementedLogic(
      ICapabilitiesImplementedDatastore datastore,
      ICapabilitiesImplementedFilter filter,
      IHttpContextAccessor context
      ) :
      base(datastore, filter, context)
    {
    }
  }
}

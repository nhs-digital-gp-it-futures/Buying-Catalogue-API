using Microsoft.AspNetCore.Http;
using NHSD.GPITF.BuyingCatalog.Interfaces;
using NHSD.GPITF.BuyingCatalog.Models;

namespace NHSD.GPITF.BuyingCatalog.Logic
{
  public sealed class StandardsApplicableLogic : ClaimsLogicBase<StandardsApplicable>, IStandardsApplicableLogic
  {
    public StandardsApplicableLogic(
      IStandardsApplicableDatastore datastore,
      IStandardsApplicableFilter filter,
      IHttpContextAccessor context) :
      base(datastore, filter, context)
    {
    }
  }
}

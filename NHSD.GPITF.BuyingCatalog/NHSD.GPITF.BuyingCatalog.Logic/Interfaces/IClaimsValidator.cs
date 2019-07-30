using NHSD.GPITF.BuyingCatalog.Models;

namespace NHSD.GPITF.BuyingCatalog.Logic
{
  public interface IClaimsValidator<in T> : IValidatorBase<T> where T : ClaimsBase
  {
  }
}

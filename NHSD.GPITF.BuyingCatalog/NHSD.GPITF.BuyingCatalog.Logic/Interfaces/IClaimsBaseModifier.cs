using NHSD.GPITF.BuyingCatalog.Models;

namespace NHSD.GPITF.BuyingCatalog.Logic
{
  public interface IClaimsBaseModifier<in T> where T : ClaimsBase
  {
    void ForUpdate(T input);
  }
}

using NHSD.GPITF.BuyingCatalog.Models;

namespace NHSD.GPITF.BuyingCatalog.Logic
{
  public interface IClaimsBaseModifier<in T> where T : ClaimsBase
  {
    void ForCreate(T input);
    void ForUpdate(T input);
  }
}

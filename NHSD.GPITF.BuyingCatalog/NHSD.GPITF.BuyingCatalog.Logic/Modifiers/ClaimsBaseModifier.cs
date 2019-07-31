using NHSD.GPITF.BuyingCatalog.Models;
using System;

namespace NHSD.GPITF.BuyingCatalog.Logic
{
  public abstract class ClaimsBaseModifier<T> : IClaimsBaseModifier<T> where T : ClaimsBase
  {
    public virtual void ForUpdate(T input)
    {
      input.OriginalDate = (input.OriginalDate == default(DateTime)) ? DateTime.UtcNow : input.OriginalDate;
    }
  }
}

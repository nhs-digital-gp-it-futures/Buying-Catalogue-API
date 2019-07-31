using System.Collections.Generic;

namespace NHSD.GPITF.BuyingCatalog.Interfaces
{
#pragma warning disable CS1591
  public interface IClaimsLogic<out T>
  {
    T ById(string id);
    IEnumerable<T> BySolution(string solutionId);
  }
#pragma warning restore CS1591
}

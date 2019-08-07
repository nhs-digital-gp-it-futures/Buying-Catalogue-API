using Microsoft.AspNetCore.Mvc;

namespace NHSD.GPITF.BuyingCatalog
{
  internal static class DefaultValues
  {
    /// <summary>
    /// Default API version
    /// </summary>
    public static ApiVersion ApiVersion => new ApiVersion(1, 0);
  }
}

namespace NHSD.GPITF.BuyingCatalog.Interfaces
{
#pragma warning restore CS1591
  public interface IShortTermCache : ICache
  {
    void ExpireValue(string path);
  }
#pragma warning restore CS1591
}


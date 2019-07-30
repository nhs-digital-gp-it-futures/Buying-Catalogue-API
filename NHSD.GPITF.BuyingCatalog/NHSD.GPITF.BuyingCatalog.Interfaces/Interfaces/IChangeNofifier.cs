namespace NHSD.GPITF.BuyingCatalog.Interfaces.Interfaces
{
#pragma warning disable CS1591
  public interface IChangeNofifier<T>
  {
    void Notify(ChangeRecord<T> record);
  }
#pragma warning restore CS1591
}

namespace NHSD.GPITF.BuyingCatalog.Interfaces.Interfaces
{
#pragma warning disable CS1591
  public sealed class ChangeRecord<T>
  {
    /// <summary>
    /// Unique identifier of Contact making change
    /// </summary>
    public string ModifierId { get; }

    public T OldVersion { get; }
    public T NewVersion { get; }

    public ChangeRecord(string modifierId, T oldVer, T newVer)
    {
      ModifierId = modifierId;
      OldVersion = oldVer;
      NewVersion = newVer;
    }
  }
#pragma warning restore CS1591
}

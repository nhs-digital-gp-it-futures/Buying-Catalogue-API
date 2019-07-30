using NHSD.GPITF.BuyingCatalog.Interfaces;

namespace NHSD.GPITF.BuyingCatalog.Models
{
  public abstract class Quality : EntityBase, IHasPreviousId
  {
    /// <summary>
    /// Unique identifier of previous version of entity
    /// </summary>
    public string PreviousId { get; set; }

    /// <summary>
    /// Name of Capability/Standard, as displayed to a user
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Description of Capability/Standard, as displayed to a user
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// URL with further information
    /// </summary>
    public string URL { get; set; }

    /// <summary>
    /// Category of Capability/Standard
    /// </summary>
    public string Type { get; set; }
  }
}

using Dapper.Contrib.Extensions;
using NHSD.GPITF.BuyingCatalog.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace NHSD.GPITF.BuyingCatalog.Models
{
  public abstract class EntityBase : IHasId
  {
    /// <summary>
    /// Unique identifier of entity
    /// </summary>
    [Required]
    [ExplicitKey]
    public string Id { get; set; }
  }
}

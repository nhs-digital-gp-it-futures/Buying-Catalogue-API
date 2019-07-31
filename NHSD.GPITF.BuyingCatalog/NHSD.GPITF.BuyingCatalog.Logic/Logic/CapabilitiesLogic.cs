using Microsoft.AspNetCore.Http;
using NHSD.GPITF.BuyingCatalog.Interfaces;
using NHSD.GPITF.BuyingCatalog.Models;
using System.Collections.Generic;
using System.Linq;

namespace NHSD.GPITF.BuyingCatalog.Logic
{
  public sealed class CapabilitiesLogic : LogicBase, ICapabilitiesLogic
  {
    private readonly ICapabilitiesDatastore _datastore;

    public CapabilitiesLogic(
      ICapabilitiesDatastore datastore, 
      IHttpContextAccessor context) :
      base(context)
    {
      _datastore = datastore;
    }

    public IEnumerable<Capabilities> ByFramework(string frameworkId)
    {
      return _datastore.ByFramework(frameworkId);
    }

    public Capabilities ById(string id)
    {
      return (new[] { _datastore.ById(id) }).SingleOrDefault();
    }

    public IEnumerable<Capabilities> ByIds(IEnumerable<string> ids)
    {
      return _datastore.ByIds(ids);
    }

    public IEnumerable<Capabilities> ByStandard(string standardId, bool isOptional)
    {
      return _datastore.ByStandard(standardId, isOptional);
    }

    public IEnumerable<Capabilities> GetAll()
    {
      return _datastore.GetAll();
    }
  }
}

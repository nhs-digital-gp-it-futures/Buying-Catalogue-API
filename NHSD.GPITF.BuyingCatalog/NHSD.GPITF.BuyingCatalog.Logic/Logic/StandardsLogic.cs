using Microsoft.AspNetCore.Http;
using NHSD.GPITF.BuyingCatalog.Interfaces;
using NHSD.GPITF.BuyingCatalog.Models;
using System.Collections.Generic;
using System.Linq;

namespace NHSD.GPITF.BuyingCatalog.Logic
{
  public sealed class StandardsLogic : LogicBase, IStandardsLogic
  {
    private readonly IStandardsDatastore _datastore;

    public StandardsLogic(
      IStandardsDatastore datastore,
      IHttpContextAccessor context) :
      base(context)
    {
      _datastore = datastore;
    }

    public IEnumerable<Standards> ByCapability(string capabilityId, bool isOptional)
    {
      return _datastore.ByCapability(capabilityId, isOptional);
    }

    public IEnumerable<Standards> ByFramework(string frameworkId)
    {
      return _datastore.ByFramework(frameworkId);
    }

    public Standards ById(string id)
    {
      return (new[] { _datastore.ById(id) }).SingleOrDefault();
    }

    public IEnumerable<Standards> ByIds(IEnumerable<string> ids)
    {
      return _datastore.ByIds(ids);
    }

    public IEnumerable<Standards> GetAll()
    {
      return _datastore.GetAll();
    }
  }
}

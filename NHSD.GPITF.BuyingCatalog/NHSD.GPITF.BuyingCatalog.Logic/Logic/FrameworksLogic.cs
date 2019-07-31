using Microsoft.AspNetCore.Http;
using NHSD.GPITF.BuyingCatalog.Interfaces;
using NHSD.GPITF.BuyingCatalog.Models;
using System.Collections.Generic;
using System.Linq;

namespace NHSD.GPITF.BuyingCatalog.Logic
{
  public sealed class FrameworksLogic : LogicBase, IFrameworksLogic
  {
    private readonly IFrameworksDatastore _datastore;

    public FrameworksLogic(
      IFrameworksDatastore datastore,
      IHttpContextAccessor context) :
      base(context)
    {
      _datastore = datastore;
    }

    public IEnumerable<Frameworks> ByCapability(string capabilityId)
    {
      return _datastore.ByCapability(capabilityId);
    }

    public IEnumerable<Frameworks> ByStandard(string standardId)
    {
      return _datastore.ByStandard(standardId);
    }

    public Frameworks ById(string id)
    {
      return (new[] { _datastore.ById(id) }).SingleOrDefault();
    }

    public IEnumerable<Frameworks> BySolution(string solutionId)
    {
      return _datastore.BySolution(solutionId);
    }

    public IEnumerable<Frameworks> GetAll()
    {
      return _datastore.GetAll();
    }
  }
}

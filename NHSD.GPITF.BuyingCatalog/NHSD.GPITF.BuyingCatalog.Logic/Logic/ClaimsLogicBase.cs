using Microsoft.AspNetCore.Http;
using NHSD.GPITF.BuyingCatalog.Interfaces;
using NHSD.GPITF.BuyingCatalog.Models;
using System.Collections.Generic;
using System.Linq;

namespace NHSD.GPITF.BuyingCatalog.Logic
{
  public abstract class ClaimsLogicBase<T> : LogicBase, IClaimsLogic<T> where T : ClaimsBase
  {
    protected readonly IClaimsDatastore<T> _datastore;
    protected readonly IClaimsFilter<T> _filter;

    protected ClaimsLogicBase(
      IClaimsDatastore<T> datastore,
      IClaimsFilter<T> filter,
      IHttpContextAccessor context) :
      base(context)
    {
      _datastore = datastore;
      _filter = filter;
    }

    public T ById(string id)
    {
      return _filter.Filter(new[] { _datastore.ById(id) }).SingleOrDefault();
    }

    public IEnumerable<T> BySolution(string solutionId)
    {
      return _filter.Filter(_datastore.BySolution(solutionId));
    }
  }
}

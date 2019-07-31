using Dapper.Contrib.Extensions;
using Microsoft.Extensions.Logging;
using NHSD.GPITF.BuyingCatalog.Datastore.Database.Interfaces;
using NHSD.GPITF.BuyingCatalog.Interfaces;
using NHSD.GPITF.BuyingCatalog.Models;
using System.Collections.Generic;
using System.Linq;

namespace NHSD.GPITF.BuyingCatalog.Datastore.Database
{
  public abstract class ClaimsDatastoreBase<T> : DatastoreBase<T>, IClaimsDatastore<ClaimsBase> where T : ClaimsBase
  {
    protected ClaimsDatastoreBase(
      IDbConnectionFactory dbConnectionFactory,
      ILogger<ClaimsDatastoreBase<T>> logger,
      ISyncPolicyFactory policy) :
      base(dbConnectionFactory, logger, policy)
    {
    }

    public T ById(string id)
    {
      return GetInternal(() =>
      {
        return _dbConnection.Get<T>(id);
      });
    }

    public IEnumerable<T> BySolution(string solutionId)
    {
      return GetInternal(() =>
      {
        return _dbConnection.GetAll<T>().Where(cc => cc.SolutionId == solutionId);
      });
    }

    public void Update(T claimedcapability)
    {
      GetInternal(() =>
      {
        using (var trans = _dbConnection.BeginTransaction())
        {
          _dbConnection.Update(claimedcapability, trans);
          trans.Commit();
          return 0;
        }
      });
    }

    public void Delete(T claimedcapability)
    {
      GetInternal(() =>
      {
        using (var trans = _dbConnection.BeginTransaction())
        {
          _dbConnection.Delete(claimedcapability, trans);
          trans.Commit();
          return 0;
        }
      });
    }

    ClaimsBase IClaimsDatastore<ClaimsBase>.ById(string id)
    {
      return ById(id);
    }

    IEnumerable<ClaimsBase> IClaimsDatastore<ClaimsBase>.BySolution(string solutionId)
    {
      return BySolution(solutionId);
    }

    void IClaimsDatastore<ClaimsBase>.Update(ClaimsBase claim)
    {
      Update((T)claim);
    }

    void IClaimsDatastore<ClaimsBase>.Delete(ClaimsBase claim)
    {
      Delete((T)claim);
    }
  }
}

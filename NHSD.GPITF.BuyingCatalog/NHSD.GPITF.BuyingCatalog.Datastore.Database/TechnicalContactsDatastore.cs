using Dapper.Contrib.Extensions;
using Microsoft.Extensions.Logging;
using NHSD.GPITF.BuyingCatalog.Datastore.Database.Interfaces;
using NHSD.GPITF.BuyingCatalog.Interfaces;
using NHSD.GPITF.BuyingCatalog.Models;
using System.Collections.Generic;
using System.Linq;

namespace NHSD.GPITF.BuyingCatalog.Datastore.Database
{
  public sealed class TechnicalContactsDatastore : DatastoreBase<TechnicalContacts>, ITechnicalContactsDatastore
  {
    public TechnicalContactsDatastore(IDbConnectionFactory dbConnectionFactory, ILogger<TechnicalContactsDatastore> logger, ISyncPolicyFactory policy) :
      base(dbConnectionFactory, logger, policy)
    {
    }

    public IEnumerable<TechnicalContacts> BySolution(string solutionId)
    {
      return GetInternal(() =>
      {
        return _dbConnection.GetAll<TechnicalContacts>().Where(tc => tc.SolutionId == solutionId);
      });
    }

    public void Update(TechnicalContacts techCont)
    {
      GetInternal(() =>
      {
        using (var trans = _dbConnection.BeginTransaction())
        {
          _dbConnection.Update(techCont, trans);
          trans.Commit();
          return 0;
        }
      });
    }

    public void Delete(TechnicalContacts techCont)
    {
      GetInternal(() =>
      {
        using (var trans = _dbConnection.BeginTransaction())
        {
          _dbConnection.Delete(techCont, trans);
          trans.Commit();
          return 0;
        }
      });
    }
  }
}

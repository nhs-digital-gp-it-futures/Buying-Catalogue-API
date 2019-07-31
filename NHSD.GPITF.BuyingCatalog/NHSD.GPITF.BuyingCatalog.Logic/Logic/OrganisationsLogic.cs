using Microsoft.AspNetCore.Http;
using NHSD.GPITF.BuyingCatalog.Interfaces;
using NHSD.GPITF.BuyingCatalog.Models;
using System.Linq;

namespace NHSD.GPITF.BuyingCatalog.Logic
{
  public sealed class OrganisationsLogic : LogicBase, IOrganisationsLogic
  {
    private readonly IOrganisationsDatastore _datastore;
    private readonly IOrganisationsFilter _filter;

    public OrganisationsLogic(
      IOrganisationsDatastore datastore,
      IHttpContextAccessor context,
      IOrganisationsFilter filter
      ) :
      base(context)
    {
      _datastore = datastore;
      _filter = filter;
    }

    public Organisations ByContact(string contactId)
    {
      return _filter.Filter(new[] { _datastore.ByContact(contactId) }).SingleOrDefault();
    }
  }
}

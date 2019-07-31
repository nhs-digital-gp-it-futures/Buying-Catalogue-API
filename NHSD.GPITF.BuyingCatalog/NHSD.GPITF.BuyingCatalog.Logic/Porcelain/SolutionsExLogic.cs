using Microsoft.AspNetCore.Http;
using NHSD.GPITF.BuyingCatalog.Interfaces;
using NHSD.GPITF.BuyingCatalog.Interfaces.Porcelain;
using NHSD.GPITF.BuyingCatalog.Models.Porcelain;
using System.Collections.Generic;
using System.Linq;

namespace NHSD.GPITF.BuyingCatalog.Logic.Porcelain
{
  public sealed class SolutionsExLogic : LogicBase, ISolutionsExLogic
  {
    private readonly ISolutionsExDatastore _datastore;
    private readonly ISolutionsExValidator _validator;
    private readonly ISolutionsExFilter _filter;
    private readonly IContactsDatastore _contacts;

    public SolutionsExLogic(
      ISolutionsExDatastore datastore,
      IHttpContextAccessor context,
      ISolutionsExValidator validator,
      ISolutionsExFilter filter,
      IContactsDatastore contacts) :
      base(context)
    {
      _datastore = datastore;
      _validator = validator;
      _filter = filter;
      _contacts = contacts;
    }

    public SolutionEx BySolution(string solutionId)
    {
      return _filter.Filter(new[] { _datastore.BySolution(solutionId) }).SingleOrDefault();
    }

    public IEnumerable<SolutionEx> ByOrganisation(string organisationId)
    {
      return _filter.Filter(_datastore.ByOrganisation(organisationId));
    }
  }
}

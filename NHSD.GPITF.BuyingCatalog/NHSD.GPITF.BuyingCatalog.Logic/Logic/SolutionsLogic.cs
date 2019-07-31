﻿using Microsoft.AspNetCore.Http;
using NHSD.GPITF.BuyingCatalog.Interfaces;
using NHSD.GPITF.BuyingCatalog.Models;
using System.Collections.Generic;
using System.Linq;

namespace NHSD.GPITF.BuyingCatalog.Logic
{
  public sealed class SolutionsLogic : LogicBase, ISolutionsLogic
  {
    private readonly ISolutionsDatastore _datastore;
    private readonly IContactsDatastore _contacts;
    private readonly ISolutionsValidator _validator;
    private readonly ISolutionsFilter _filter;

    public SolutionsLogic(
      ISolutionsDatastore datastore,
      IContactsDatastore contacts,
      IHttpContextAccessor context,
      ISolutionsValidator validator,
      ISolutionsFilter filter) :
      base(context)
    {
      _datastore = datastore;
      _contacts = contacts;
      _validator = validator;
      _filter = filter;
    }

    public IEnumerable<Solutions> ByFramework(string frameworkId)
    {
      return _filter.Filter(_datastore.ByFramework(frameworkId));
    }

    public Solutions ById(string id)
    {
      return _filter.Filter(new[] { _datastore.ById(id) }).SingleOrDefault();
    }

    public IEnumerable<Solutions> ByOrganisation(string organisationId)
    {
      return _filter.Filter(_datastore.ByOrganisation(organisationId));
    }
  }
}

﻿using Microsoft.AspNetCore.Http;
using NHSD.GPITF.BuyingCatalog.Interfaces;
using NHSD.GPITF.BuyingCatalog.Models;
using System.Collections.Generic;

namespace NHSD.GPITF.BuyingCatalog.Logic
{
  public sealed class TechnicalContactsLogic : LogicBase, ITechnicalContactsLogic
  {
    private readonly ITechnicalContactsDatastore _datastore;
    private readonly ITechnicalContactsFilter _filter;

    public TechnicalContactsLogic(
      ITechnicalContactsDatastore datastore,
      IHttpContextAccessor context,
      ITechnicalContactsFilter filter) :
      base(context)
    {
      _datastore = datastore;
      _filter = filter;
    }

    public IEnumerable<TechnicalContacts> BySolution(string solutionId)
    {
      return _filter.Filter(_datastore.BySolution(solutionId));
    }
  }
}

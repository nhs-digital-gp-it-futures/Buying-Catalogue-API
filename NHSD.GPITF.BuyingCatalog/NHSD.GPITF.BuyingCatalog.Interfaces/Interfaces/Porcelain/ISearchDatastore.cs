﻿using NHSD.GPITF.BuyingCatalog.Models.Porcelain;
using System.Collections.Generic;

namespace NHSD.GPITF.BuyingCatalog.Interfaces.Porcelain
{
#pragma warning disable CS1591
  public interface ISearchDatastore
  {
    IEnumerable<SearchResult> ByCapabilities(IEnumerable<string> capIds);
  }
#pragma warning restore CS1591
}

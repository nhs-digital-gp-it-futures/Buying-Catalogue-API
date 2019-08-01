using Microsoft.AspNetCore.Http;
using NHSD.GPITF.BuyingCatalog.Interfaces;
using NHSD.GPITF.BuyingCatalog.Models;
using System.Linq;

namespace NHSD.GPITF.BuyingCatalog.Logic
{
  public sealed class TechnicalContactsFilter : FilterBase<TechnicalContacts>, ITechnicalContactsFilter
  {
    private readonly ISolutionsDatastore _solutionDatastore;
    private readonly ISolutionsFilter _solutionsFilter;

    public TechnicalContactsFilter(
      IHttpContextAccessor context,
      ISolutionsDatastore solutionDatastore,
      ISolutionsFilter solutionsFilter) :
      base(context)
    {
      _solutionDatastore = solutionDatastore;
      _solutionsFilter = solutionsFilter;
    }

    public override TechnicalContacts Filter(TechnicalContacts input)
    {
      var soln = _solutionDatastore.ById(input.SolutionId);

      if (_context.HasRole(Roles.Admin))
      {
        // Admin:  everything
        return input;
      }

      if (_context.HasRole(Roles.Supplier))
      {
        // Supplier: only own TechnicalContacts
        return _context.OrganisationId() == soln.OrganisationId ? input : null;
      }

      if (_context.HasRole(Roles.Buyer))
      {
        // Buyer: hide draft & failed Solutions
        var buyerSoln = _solutionsFilter.Filter(new[] { _solutionDatastore.ById(input.SolutionId) }).SingleOrDefault();
        if (buyerSoln == null)
        {
          return null;
        }

        return input;
      }

      // None:  only approved solutions
      var noneSoln = _solutionsFilter.Filter(new[] { _solutionDatastore.ById(input.SolutionId) }).SingleOrDefault();
      if (noneSoln == null)
      {
        return null;
      }

      return input;
    }
  }
}

using Microsoft.AspNetCore.Http;
using NHSD.GPITF.BuyingCatalog.Interfaces;
using NHSD.GPITF.BuyingCatalog.Models;
using System.Linq;

namespace NHSD.GPITF.BuyingCatalog.Logic
{
  public abstract class ClaimsFilterBase<T> : FilterBase<T>, IClaimsFilter<T> where T : ClaimsBase
  {
    private readonly ISolutionsDatastore _solutionDatastore;
    private readonly ISolutionsFilter _solutionsFilter;

    protected ClaimsFilterBase(
      IHttpContextAccessor context,
      ISolutionsDatastore solutionDatastore,
      ISolutionsFilter solutionsFilter) :
      base(context)
    {
      _solutionDatastore = solutionDatastore;
      _solutionsFilter = solutionsFilter;
    }

    protected virtual T FilterSpecific(T input)
    {
      return input;
    }

    public override T Filter(T input)
    {
      if (_context.HasRole(Roles.Admin))
      {
        input = FilterForAdmin(input);
      }
      else if (_context.HasRole(Roles.Buyer))
      {
        input = FilterForBuyer(input);
      }
      else if (_context.HasRole(Roles.Supplier))
      {
        input = FilterForSupplier(input);
      }
      else
      {
        input = FilterForNone(input);
      }

      return FilterSpecific(input);
    }

    public T FilterForAdmin(T input)
    {
      // Admin: everything
      return input;
    }

    public T FilterForBuyer(T input)
    {
      // Buyer: hide draft & failed Solutions
      var buyerSoln = _solutionsFilter.Filter(new[] { _solutionDatastore.ById(input.SolutionId) }).SingleOrDefault();
      if (buyerSoln == null)
      {
        return null;
      }

      return input;
    }

    public T FilterForSupplier(T input)
    {
      // Supplier: only own Claims
      var soln = _solutionDatastore.ById(input.SolutionId);
      return _context.OrganisationId() == soln?.OrganisationId ? input : null;
    }

    public T FilterForNone(T input)
    {
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

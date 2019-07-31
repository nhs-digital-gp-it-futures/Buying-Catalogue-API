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
    private readonly ISolutionsModifier _solutionsModifier;

    private readonly ICapabilitiesImplementedModifier _capabilitiesImplementedModifier;
    private readonly IStandardsApplicableModifier _standardsApplicableModifier;

    private readonly ISolutionsExDatastore _datastore;
    private readonly ISolutionsExValidator _validator;
    private readonly ISolutionsExFilter _filter;
    private readonly IContactsDatastore _contacts;

    public SolutionsExLogic(
      ISolutionsModifier solutionsModifier,

      ICapabilitiesImplementedModifier capabilitiesImplementedModifier,
      IStandardsApplicableModifier standardsApplicableModifier,

      ISolutionsExDatastore datastore,
      IHttpContextAccessor context,
      ISolutionsExValidator validator,
      ISolutionsExFilter filter,
      IContactsDatastore contacts) :
      base(context)
    {
      _solutionsModifier = solutionsModifier;

      _capabilitiesImplementedModifier = capabilitiesImplementedModifier;
      _standardsApplicableModifier = standardsApplicableModifier;

      _datastore = datastore;
      _validator = validator;
      _filter = filter;
      _contacts = contacts;
    }

    public SolutionEx BySolution(string solutionId)
    {
      return _filter.Filter(new[] { _datastore.BySolution(solutionId) }).SingleOrDefault();
    }

    public void Update(SolutionEx solnEx)
    {
      _validator.ValidateAndThrowEx(solnEx, ruleSet: nameof(ISolutionsExLogic.Update));

      _solutionsModifier.ForUpdate(solnEx.Solution);

      solnEx.ClaimedCapability.ForEach(claim => _capabilitiesImplementedModifier.ForUpdate(claim));
      solnEx.ClaimedStandard.ForEach(claim => _standardsApplicableModifier.ForUpdate(claim));

      _datastore.Update(solnEx);
    }

    public IEnumerable<SolutionEx> ByOrganisation(string organisationId)
    {
      return _filter.Filter(_datastore.ByOrganisation(organisationId));
    }
  }
}

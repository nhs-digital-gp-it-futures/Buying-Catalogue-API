using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NHSD.GPITF.BuyingCatalog.Attributes;
using NHSD.GPITF.BuyingCatalog.Interfaces;
using NHSD.GPITF.BuyingCatalog.Models;
using System.ComponentModel.DataAnnotations;
using System.Net;

namespace NHSD.GPITF.BuyingCatalog.Controllers
{
  /// <summary>
  /// Manage claimed standards
  /// </summary>
  [ApiVersion("1")]
  [Route("api/v{version:apiVersion}/[controller]")]
  [AllowAnonymous]
  [Produces("application/json")]
  public sealed class StandardsApplicableController : Controller
  {
    private readonly IStandardsApplicableLogic _logic;

    /// <summary>
    /// constructor for ClaimedStandardController
    /// </summary>
    /// <param name="logic">business logic</param>
    public StandardsApplicableController(IStandardsApplicableLogic logic)
    {
      _logic = logic;
    }

    /// <summary>
    /// Retrieve claim, given the claim’s CRM identifier
    /// </summary>
    /// <param name="id">CRM identifier of claim</param>
    /// <response code="200">Success</response>
    /// <response code="404">Claim not found in CRM</response>
    [HttpGet]
    [Route("{id}")]
    [ValidateModelState]
    [ProducesResponseType(statusCode: (int)HttpStatusCode.OK, type: typeof(StandardsApplicable))]
    [ProducesResponseType(statusCode: (int)HttpStatusCode.NotFound)]
    public IActionResult ById([FromRoute][Required]string id)
    {
      var claim = _logic.ById(id);
      return claim != null ? (IActionResult)new OkObjectResult(claim) : new NotFoundResult();
    }

    /// <summary>
    /// Retrieve all claimed standards for a solution in a paged list,
    ///  given the solution’s CRM identifier
    /// </summary>
    /// <param name="solutionId">CRM identifier of solution</param>
    /// <param name="pageIndex">1-based index of page to return.  Defaults to 1</param>
    /// <param name="pageSize">number of items per page.  Defaults to 20</param>
    /// <response code="200">Success</response>
    /// <response code="404">Solution not found in CRM</response>
    [HttpGet]
    [Route("BySolution/{solutionId}")]
    [ValidateModelState]
    [ProducesResponseType(statusCode: (int)HttpStatusCode.OK, type: typeof(PaginatedList<StandardsApplicable>))]
    [ProducesResponseType(statusCode: (int)HttpStatusCode.NotFound)]
    public IActionResult BySolution([FromRoute][Required]string solutionId, [FromQuery]int? pageIndex, [FromQuery]int? pageSize)
    {
      var stds = _logic.BySolution(solutionId);
      var retval = PaginatedList<StandardsApplicable>.Create(stds, pageIndex, pageSize);

      return new OkObjectResult(retval);
    }
  }
}

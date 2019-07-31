using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NHSD.GPITF.BuyingCatalog.Attributes;
using NHSD.GPITF.BuyingCatalog.Interfaces;
using NHSD.GPITF.BuyingCatalog.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.ComponentModel.DataAnnotations;
using System.Net;
using ZNetCS.AspNetCore.Authentication.Basic;

namespace NHSD.GPITF.BuyingCatalog.Controllers
{
  /// <summary>
  /// Create, find and retrieve solutions
  /// </summary>
  [ApiVersion("1")]
  [Route("api/[controller]")]
  [Authorize(
    Roles = Roles.Admin + "," + Roles.Buyer + "," + Roles.Supplier,
    AuthenticationSchemes = BasicAuthenticationDefaults.AuthenticationScheme)]
  [Produces("application/json")]
  public sealed class SolutionsController : Controller
  {
    private readonly ISolutionsLogic _logic;

    /// <summary>
    /// constructor for SolutionController
    /// </summary>
    /// <param name="logic">business logic</param>
    public SolutionsController(ISolutionsLogic logic)
    {
      _logic = logic;
    }

    /// <summary>
    /// Get existing solution/s on which were onboarded onto a framework,
    /// given the CRM identifier of the framework
    /// </summary>
    /// <param name="frameworkId">CRM identifier of framework</param>
    /// <param name="pageIndex">1-based index of page to return.  Defaults to 1</param>
    /// <param name="pageSize">number of items per page.  Defaults to 20</param>
    /// <response code="200">Success</response>
    /// <response code="404">Framework not found in CRM</response>
    [HttpGet]
    [Route("ByFramework/{frameworkId}")]
    [ValidateModelState]
    [SwaggerResponse(statusCode: (int)HttpStatusCode.OK, type: typeof(PaginatedList<Solutions>), description: "Success")]
    [SwaggerResponse(statusCode: (int)HttpStatusCode.NotFound, description: "Framework not found in CRM")]
    public IActionResult ByFramework([FromRoute][Required]string frameworkId, [FromQuery]int? pageIndex, [FromQuery]int? pageSize)
    {
      var solutions = _logic.ByFramework(frameworkId);
      var retval = PaginatedList<Solutions>.Create(solutions, pageIndex, pageSize);

      return new OkObjectResult(retval);
    }

    /// <summary>
    /// Get an existing solution given its CRM identifier
    /// Typically used to retrieve previous version
    /// </summary>
    /// <param name="id">CRM identifier of solution to find</param>
    /// <response code="200">Success</response>
    /// <response code="404">Solution not found in CRM</response>
    [HttpGet]
    [Route("ById/{id}")]
    [ValidateModelState]
    [SwaggerResponse(statusCode: (int)HttpStatusCode.OK, type: typeof(Solutions), description: "Success")]
    [SwaggerResponse(statusCode: (int)HttpStatusCode.NotFound, description: "Solution not found in CRM")]
    public IActionResult ById([FromRoute][Required]string id)
    {
      var solution = _logic.ById(id);
      return solution != null ? (IActionResult)new OkObjectResult(solution) : new NotFoundResult();
    }

    /// <summary>
    /// Retrieve all current solutions in a paged list for an organisation,
    /// given the organisation’s CRM identifier
    /// </summary>
    /// <param name="organisationId">CRM identifier of organisation</param>
    /// <param name="pageIndex">1-based index of page to return.  Defaults to 1</param>
    /// <param name="pageSize">number of items per page.  Defaults to 20</param>
    /// <response code="200">Success</response>
    [HttpGet]
    [Route("ByOrganisation/{organisationId}")]
    [ValidateModelState]
    [SwaggerResponse(statusCode: 200, type: typeof(PaginatedList<Solutions>), description: "Success")]
    [SwaggerResponse(statusCode: (int)HttpStatusCode.OK, type: typeof(PaginatedList<Solutions>), description: "Success")]
    public IActionResult ByOrganisation([FromRoute][Required]string organisationId, [FromQuery]int? pageIndex, [FromQuery]int? pageSize)
    {
      var solutions = _logic.ByOrganisation(organisationId);
      var retval = PaginatedList<Solutions>.Create(solutions, pageIndex, pageSize);
      return new OkObjectResult(retval);
    }
  }
}

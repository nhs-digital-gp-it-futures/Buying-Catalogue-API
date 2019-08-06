using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NHSD.GPITF.BuyingCatalog.Attributes;
using NHSD.GPITF.BuyingCatalog.Interfaces;
using NHSD.GPITF.BuyingCatalog.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net;

namespace NHSD.GPITF.BuyingCatalog.Controllers
{
  /// <summary>
  /// Manage standards
  /// </summary>
  [ApiVersion("1")]
  [Route("api/v{version:apiVersion}/[controller]")]
  [AllowAnonymous]
  [Produces("application/json")]
  public sealed class StandardsController : Controller
  {
    private readonly IStandardsLogic _logic;

    /// <summary>
    /// constructor for StandardController
    /// </summary>
    /// <param name="logic">business logic</param>
    public StandardsController(IStandardsLogic logic)
    {
      _logic = logic;
    }

    /// <summary>
    /// Get existing/optional Standard/s which are in the given Capability
    /// </summary>
    /// <param name="capabilityId">CRM identifier of Capability</param>
    /// <param name="isOptional">true if the specified Standard is optional with the Capability</param>
    /// <param name="pageIndex">1-based index of page to return.  Defaults to 1</param>
    /// <param name="pageSize">number of items per page.  Defaults to 20</param>
    /// <response code="200">Success</response>
    /// <response code="404">Capability not found in CRM</response>
    [HttpGet]
    [Route("ByCapability/{capabilityId}")]
    [ValidateModelState]
    [ProducesResponseType(statusCode: (int)HttpStatusCode.OK, type: typeof(PaginatedList<Standards>))]
    [ProducesResponseType(statusCode: (int)HttpStatusCode.NotFound)]
    public IActionResult ByCapability([FromRoute][Required]string capabilityId, [FromQuery][Required]bool isOptional, [FromQuery]int? pageIndex, [FromQuery]int? pageSize)
    {
      var stds = _logic.ByCapability(capabilityId, isOptional);
      var retval = PaginatedList<Standards>.Create(stds, pageIndex, pageSize);

      return new OkObjectResult(retval);
    }

    /// <summary>
    /// Get existing Standard/s which are in the given Framework
    /// </summary>
    /// <param name="frameworkId">CRM identifier of Framework</param>
    /// <param name="pageIndex">1-based index of page to return.  Defaults to 1</param>
    /// <param name="pageSize">number of items per page.  Defaults to 20</param>
    /// <response code="200">Success</response>
    /// <response code="404">Framework not found in CRM</response>
    [HttpGet]
    [Route("ByFramework/{frameworkId}")]
    [ValidateModelState]
    [ProducesResponseType(statusCode: (int)HttpStatusCode.OK, type: typeof(PaginatedList<Standards>))]
    [ProducesResponseType(statusCode: (int)HttpStatusCode.NotFound)]
    public IActionResult ByFramework([FromRoute][Required]string frameworkId, [FromQuery]int? pageIndex, [FromQuery]int? pageSize)
    {
      var stds = _logic.ByFramework(frameworkId);
      var retval = PaginatedList<Standards>.Create(stds, pageIndex, pageSize);

      return new OkObjectResult(retval);
    }

    /// <summary>
    /// Get an existing standard given its CRM identifier
    /// Typically used to retrieve previous version
    /// </summary>
    /// <param name="id">CRM identifier of standard to find</param>
    /// <response code="200">Success</response>
    /// <response code="404">Standard not found in CRM</response>
    [HttpGet]
    [Route("ById/{id}")]
    [ValidateModelState]
    [ProducesResponseType(statusCode: (int)HttpStatusCode.OK, type: typeof(Standards))]
    [ProducesResponseType(statusCode: (int)HttpStatusCode.NotFound)]
    public IActionResult ById([FromRoute][Required]string id)
    {
      var std = _logic.ById(id);
      return std != null ? (IActionResult)new OkObjectResult(std) : new NotFoundResult();
    }

    /// <summary>
    /// Get several existing Standards given their CRM identifiers
    /// </summary>
    /// <param name="ids">Array of CRM identifiers of Standards to find</param>
    /// <response code="200">Success</response>
    /// <response code="404">Standards not found in CRM</response>
    [HttpPost]
    [Route("ByIds")]
    [ValidateModelState]
    [ProducesResponseType(statusCode: (int)HttpStatusCode.OK, type: typeof(IEnumerable<Standards>))]
    [ProducesResponseType(statusCode: (int)HttpStatusCode.NotFound)]
    public IActionResult ByIds([FromBody][Required]IEnumerable<string> ids)
    {
      var stds = _logic.ByIds(ids);

      return new OkObjectResult(stds);
    }

    /// <summary>
    /// Retrieve all current standards in a paged list
    /// </summary>
    /// <param name="pageIndex">1-based index of page to return.  Defaults to 1</param>
    /// <param name="pageSize">number of items per page.  Defaults to 20</param>
    /// <response code="200">Success - if no standards found, return empty list</response>
    [HttpGet]
    [ValidateModelState]
    [ProducesResponseType(statusCode: (int)HttpStatusCode.OK, type: typeof(PaginatedList<Standards>))]
    public IActionResult Get([FromQuery]int? pageIndex, [FromQuery]int? pageSize)
    {
      var allStds = _logic.GetAll();
      var retval = PaginatedList<Standards>.Create(allStds, pageIndex, pageSize);
      return new OkObjectResult(retval);
    }
  }
}

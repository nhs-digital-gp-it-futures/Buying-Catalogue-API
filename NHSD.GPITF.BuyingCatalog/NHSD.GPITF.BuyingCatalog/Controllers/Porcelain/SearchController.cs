using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NHSD.GPITF.BuyingCatalog.Attributes;
using NHSD.GPITF.BuyingCatalog.Interfaces.Porcelain;
using NHSD.GPITF.BuyingCatalog.Models;
using NHSD.GPITF.BuyingCatalog.Models.Porcelain;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net;

namespace NHSD.GPITF.BuyingCatalog.Controllers.Porcelain
{
  /// <summary>
  /// Find solutions in the system
  /// </summary>
  [ApiVersion("1")]
  [ApiTag("porcelain")]
  [Route("api/porcelain/[controller]")]
  [AllowAnonymous]
  [Produces("application/json")]
  public sealed class SearchController : Controller
  {
    private readonly ISearchLogic _logic;

    /// <summary>
    /// constructor
    /// </summary>
    /// <param name="logic">business logic</param>
    public SearchController(ISearchLogic logic)
    {
      _logic = logic;
    }

    /// <summary>
    /// Get existing solution/s which support at least *all* of the specified capabilities
    /// </summary>
    /// <param name="capIds">list of capability ids</param>
    /// <param name="pageIndex">1-based index of page to return.  Defaults to 1</param>
    /// <param name="pageSize">number of items per page.  Defaults to 20</param>
    /// <response code="200">Success</response>
    [HttpPost]
    [Route("ByCapabilities")]
    [ValidateModelState]
    [AllowAnonymous]
    [ProducesResponseType(statusCode: (int)HttpStatusCode.OK, type: typeof(PaginatedList<SolutionEx>))]
    public IActionResult ByCapabilities([FromBody][Required] IEnumerable<string> capIds, [FromQuery]int? pageIndex, [FromQuery]int? pageSize)
    {
      var solutions = _logic.ByCapabilities(capIds);
      var retval = PaginatedList<SolutionEx>.Create(solutions, pageIndex, pageSize);
      return new OkObjectResult(retval);
    }
  }
}

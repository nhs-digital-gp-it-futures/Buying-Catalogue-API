﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPITF.BuyingCatalog.Attributes;
using NHSD.GPITF.BuyingCatalog.Interfaces.Porcelain;
using NHSD.GPITF.BuyingCatalog.Models.Porcelain;
using System.Net;

namespace NHSD.GPITF.BuyingCatalog.Controllers.Porcelain
{
  /// <summary>
  /// Access to capabilities with a list of corresponding, optional standards
  /// </summary>
  [ApiVersion("1")]
  [ApiTag("porcelain")]
  [Route("api/v{version:apiVersion}/porcelain/[controller]")]
  [AllowAnonymous]
  [Produces("application/json")]
  public sealed class CapabilityMappingsController : Controller
  {
    private readonly ICapabilityMappingsLogic _logic;

    /// <summary>
    /// constructor
    /// </summary>
    /// <param name="logic">business logic</param>
    public CapabilityMappingsController(ICapabilityMappingsLogic logic)
    {
      _logic = logic;
    }

    /// <summary>
    /// Get capabilities with a list of corresponding, optional standards
    /// </summary>
    /// <response code="200">Success</response>
    [HttpGet]
    [ValidateModelState]
    [ProducesResponseType(statusCode: (int)HttpStatusCode.OK, type: typeof(CapabilityMappings))]
    public IActionResult Get()
    {
      var capMaps = _logic.GetAll();
      return new OkObjectResult(capMaps);
    }
  }
}

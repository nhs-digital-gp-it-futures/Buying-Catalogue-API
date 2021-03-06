﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPITF.BuyingCatalog.Attributes;
using NHSD.GPITF.BuyingCatalog.Interfaces.Porcelain;
using NHSD.GPITF.BuyingCatalog.Models.Porcelain;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net;

namespace NHSD.GPITF.BuyingCatalog.Controllers.Porcelain
{
  /// <summary>
  /// Access to an Extended Solution with its corresponding Technical Contacts, ClaimedCapability, ClaimedStandard et al
  /// </summary>
  [ApiVersion("1")]
  [ApiTag("porcelain")]
  [Route("api/v{version:apiVersion}/porcelain/[controller]")]
  [AllowAnonymous]
  [Produces("application/json")]
  public sealed class SolutionsExController : Controller
  {
    private readonly ISolutionsExLogic _logic;

    /// <summary>
    /// constructor
    /// </summary>
    /// <param name="logic">business logic</param>
    public SolutionsExController(ISolutionsExLogic logic)
    {
      _logic = logic;
    }

    /// <summary>
    /// Get a Solution with a list of corresponding TechnicalContact, ClaimedCapability, ClaimedStandard et al
    /// </summary>
    /// <param name="solutionId">CRM identifier of Solution</param>
    /// <response code="200">Success</response>
    [HttpGet]
    [Route("BySolution/{solutionId}")]
    [ValidateModelState]
    [ProducesResponseType(statusCode: (int)HttpStatusCode.OK, type: typeof(SolutionEx))]
    [ProducesResponseType(statusCode: (int)HttpStatusCode.NotFound)]
    public IActionResult BySolution([FromRoute][Required]string solutionId)
    {
      var solnEx = _logic.BySolution(solutionId);

      return solnEx?.Solution != null ? (IActionResult)new OkObjectResult(solnEx) : new NotFoundResult();
    }

    /// <summary>
    /// Get a list of Solutions, each with a list of corresponding TechnicalContact, ClaimedCapability, ClaimedStandard et al
    /// </summary>
    /// <param name="organisationId">CRM identifier of Organisation</param>
    /// <response code="200">Success</response>
    [HttpGet]
    [Route("ByOrganisation/{organisationId}")]
    [ValidateModelState]
    [ProducesResponseType(statusCode: (int)HttpStatusCode.OK, type: typeof(IEnumerable<SolutionEx>))]
    public IActionResult ByOrganisation([FromRoute][Required]string organisationId)
    {
      var solnExs = _logic.ByOrganisation(organisationId);

      return new OkObjectResult(solnExs);
    }
  }
}

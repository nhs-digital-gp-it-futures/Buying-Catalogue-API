using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Moq;
using NHSD.GPITF.BuyingCatalog.Interfaces;
using NHSD.GPITF.BuyingCatalog.Interfaces.Porcelain;
using NHSD.GPITF.BuyingCatalog.Logic.Porcelain;
using NHSD.GPITF.BuyingCatalog.Models;
using NHSD.GPITF.BuyingCatalog.Models.Porcelain;
using NHSD.GPITF.BuyingCatalog.Tests;
using NUnit.Framework;
using System.Collections.Generic;

namespace NHSD.GPITF.BuyingCatalog.Logic.Tests.Porcelain
{
  [TestFixture]
  public sealed class SolutionsExLogic_Tests
  {
    private Mock<ISolutionsModifier> _solutionsModifier;

    private Mock<ICapabilitiesImplementedModifier> _capabilitiesImplementedModifier;
    private Mock<IStandardsApplicableModifier> _standardsApplicableModifier;

    private Mock<ISolutionsExDatastore> _datastore;
    private Mock<IContactsDatastore> _contacts;
    private Mock<IHttpContextAccessor> _context;
    private Mock<ISolutionsExValidator> _validator;
    private Mock<ISolutionsExFilter> _filter;

    [SetUp]
    public void SetUp()
    {
      _solutionsModifier = new Mock<ISolutionsModifier>();

      _capabilitiesImplementedModifier = new Mock<ICapabilitiesImplementedModifier>();
      _standardsApplicableModifier = new Mock<IStandardsApplicableModifier>();

      _datastore = new Mock<ISolutionsExDatastore>();
      _contacts = new Mock<IContactsDatastore>();
      _context = new Mock<IHttpContextAccessor>();
      _validator = new Mock<ISolutionsExValidator>();
      _filter = new Mock<ISolutionsExFilter>();
    }

    [Test]
    public void Constructor_Completes()
    {
      Assert.DoesNotThrow(() => new SolutionsExLogic(
        _solutionsModifier.Object,
        _capabilitiesImplementedModifier.Object,
        _standardsApplicableModifier.Object,
        _datastore.Object, _context.Object, _validator.Object, _filter.Object,
        _contacts.Object));
    }

    [Test]
    public void Update_CallsValidator_WithRuleset()
    {
      var logic = new SolutionsExLogic(
        _solutionsModifier.Object,
        _capabilitiesImplementedModifier.Object,
        _standardsApplicableModifier.Object,
        _datastore.Object, _context.Object, _validator.Object, _filter.Object,
        _contacts.Object);
      var solnEx = Creator.GetSolutionEx();
      _context.Setup(x => x.HttpContext).Returns(Creator.GetContext());
      _contacts.Setup(x => x.ByEmail(It.IsAny<string>())).Returns(Creator.GetContact());

      var valres = new ValidationResult();
      _validator.Setup(x => x.Validate(It.IsAny<ValidationContext>())).Returns(valres);

      logic.Update(solnEx);

      _validator.Verify(x => x.ValidateAndThrowEx(
        It.Is<SolutionEx>(sex => sex == solnEx),
        It.Is<string>(rs => rs == nameof(ISolutionsExLogic.Update))), Times.Once());
    }

    [Test]
    public void Update_Calls_SolutionModifier()
    {
      var logic = new SolutionsExLogic(
        _solutionsModifier.Object,
        _capabilitiesImplementedModifier.Object,
        _standardsApplicableModifier.Object,
        _datastore.Object, _context.Object, _validator.Object, _filter.Object,
        _contacts.Object);
      var soln = Creator.GetSolution();
      var solnEx = Creator.GetSolutionEx(soln: soln);

      var valres = new ValidationResult();
      _validator.Setup(x => x.Validate(It.IsAny<ValidationContext>())).Returns(valres);

      logic.Update(solnEx);

      _solutionsModifier.Verify(x => x.ForUpdate(soln), Times.Once);
    }

    [Test]
    public void Update_Calls_Modifier_For_ClaimedCapability()
    {
      var logic = new SolutionsExLogic(
        _solutionsModifier.Object,
        _capabilitiesImplementedModifier.Object,
        _standardsApplicableModifier.Object,
        _datastore.Object, _context.Object, _validator.Object, _filter.Object,
        _contacts.Object);
      var claim = Creator.GetCapabilitiesImplemented();
      var soln = Creator.GetSolution();
      var solnEx = Creator.GetSolutionEx(soln: soln, claimedCap: new List<CapabilitiesImplemented>(new[] { claim }));

      var valres = new ValidationResult();
      _validator.Setup(x => x.Validate(It.IsAny<ValidationContext>())).Returns(valres);

      logic.Update(solnEx);

      _capabilitiesImplementedModifier.Verify(x => x.ForUpdate(claim), Times.Once);
    }

    [Test]
    public void Update_Calls_Modifier_For_ClaimedStandard()
    {
      var logic = new SolutionsExLogic(
        _solutionsModifier.Object,
        _capabilitiesImplementedModifier.Object,
        _standardsApplicableModifier.Object,
        _datastore.Object, _context.Object, _validator.Object, _filter.Object,
        _contacts.Object);
      var claim = Creator.GetStandardsApplicable();
      var soln = Creator.GetSolution();
      var solnEx = Creator.GetSolutionEx(soln: soln, claimedStd: new List<StandardsApplicable>(new[] { claim }));

      var valres = new ValidationResult();
      _validator.Setup(x => x.Validate(It.IsAny<ValidationContext>())).Returns(valres);

      logic.Update(solnEx);

      _standardsApplicableModifier.Verify(x => x.ForUpdate(claim), Times.Once);
    }
  }
}

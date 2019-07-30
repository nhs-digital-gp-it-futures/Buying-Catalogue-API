﻿using FluentValidation;
using FluentValidation.Internal;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Moq;
using NHSD.GPITF.BuyingCatalog.Interfaces;
using NHSD.GPITF.BuyingCatalog.Interfaces.Interfaces;
using NHSD.GPITF.BuyingCatalog.Models;
using NHSD.GPITF.BuyingCatalog.Tests;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace NHSD.GPITF.BuyingCatalog.Logic.Tests
{
  [TestFixture]
  public sealed class SolutionsLogic_Tests
  {
    private Mock<ISolutionsModifier> _modifier;
    private Mock<ISolutionsDatastore> _datastore;
    private Mock<IContactsDatastore> _contacts;
    private Mock<IHttpContextAccessor> _context;
    private Mock<ISolutionsValidator> _validator;
    private Mock<ISolutionsFilter> _filter;
    private Mock<IEvidenceBlobStoreLogic> _evidenceBlobStoreLogic;
    private Mock<ISolutionsChangeNotifier> _notifier;

    [SetUp]
    public void SetUp()
    {
      _modifier = new Mock<ISolutionsModifier>();
      _datastore = new Mock<ISolutionsDatastore>();
      _contacts = new Mock<IContactsDatastore>();
      _context = new Mock<IHttpContextAccessor>();
      _validator = new Mock<ISolutionsValidator>();
      _filter = new Mock<ISolutionsFilter>();
      _evidenceBlobStoreLogic = new Mock<IEvidenceBlobStoreLogic>();
      _notifier = new Mock<ISolutionsChangeNotifier>();
    }

    [Test]
    public void Constructor_Completes()
    {
      Assert.DoesNotThrow(() => Create());
    }

    [Test]
    public void ByFramework_CallsFilter()
    {
      var logic = Create();

      logic.ByFramework("some Id");

      _filter.Verify(x => x.Filter(It.IsAny<IEnumerable<Solutions>>()), Times.Once());
    }

    [Test]
    public void ById_CallsFilter()
    {
      var logic = Create();

      logic.ById("some Id");

      _filter.Verify(x => x.Filter(It.IsAny<IEnumerable<Solutions>>()), Times.Once());
    }

    [Test]
    public void ByOrganisation_CallsFilter()
    {
      var logic = Create();

      logic.ByOrganisation("some Id");

      _filter.Verify(x => x.Filter(It.IsAny<IEnumerable<Solutions>>()), Times.Once());
    }

    [Test]
    public void Create_CallsValidator_WithRuleset()
    {
      var logic = Create();
      var soln = Creator.GetSolution();
      _context.Setup(x => x.HttpContext).Returns(Creator.GetContext());
      _contacts.Setup(x => x.ByEmail(It.IsAny<string>())).Returns(Creator.GetContact());

      var valres = new ValidationResult();
      _validator.Setup(x => x.Validate(It.IsAny<ValidationContext>())).Returns(valres);

      logic.Create(soln);

      _validator.Verify(x => x.ValidateAndThrowEx(
        It.Is<Solutions>(s => s == soln),
        It.Is<string>(rs => rs == nameof(ISolutionsLogic.Create))), Times.Once());
    }

    [Test]
    public void Update_CallsValidator_WithRuleset()
    {
      var logic = Create();
      var soln = Creator.GetSolution();
      _context.Setup(x => x.HttpContext).Returns(Creator.GetContext());
      _contacts.Setup(x => x.ByEmail(It.IsAny<string>())).Returns(Creator.GetContact());

      var valres = new ValidationResult();
      _validator.Setup(x => x.Validate(It.IsAny<ValidationContext>())).Returns(valres);

      logic.Update(soln);

      _validator.Verify(x => x.ValidateAndThrowEx(
        It.Is<Solutions>(s => s == soln),
        It.Is<string>(rs => rs == nameof(ISolutionsLogic.Update))), Times.Once());
    }

    [TestCase(SolutionStatus.Registered)]
    public void Update_CallsPrepareForSolution_WhenRegistered(SolutionStatus status)
    {
      var logic = Create();
      var soln = Creator.GetSolution(status: status);
      _context.Setup(x => x.HttpContext).Returns(Creator.GetContext());
      _contacts.Setup(x => x.ByEmail(It.IsAny<string>())).Returns(Creator.GetContact());

      var valres = new ValidationResult();
      _validator.Setup(x => x.Validate(It.IsAny<ValidationContext>())).Returns(valres);

      logic.Update(soln);

      _evidenceBlobStoreLogic.Verify(x => x.PrepareForSolution(soln.Id), Times.Once);
    }

    [TestCase(SolutionStatus.Failed)]
    [TestCase(SolutionStatus.Draft)]
    [TestCase(SolutionStatus.CapabilitiesAssessment)]
    [TestCase(SolutionStatus.StandardsCompliance)]
    [TestCase(SolutionStatus.FinalApproval)]
    [TestCase(SolutionStatus.SolutionPage)]
    [TestCase(SolutionStatus.Approved)]
    public void Update_DoesNotCallPrepareForSolution_WhenNotRegistered(SolutionStatus status)
    {
      var logic = Create();
      var soln = Creator.GetSolution(status: status);
      _context.Setup(x => x.HttpContext).Returns(Creator.GetContext());
      _contacts.Setup(x => x.ByEmail(It.IsAny<string>())).Returns(Creator.GetContact());

      var valres = new ValidationResult();
      _validator.Setup(x => x.Validate(It.IsAny<ValidationContext>())).Returns(valres);

      logic.Update(soln);

      _evidenceBlobStoreLogic.Verify(x => x.PrepareForSolution(soln.Id), Times.Never);
    }

    [Test]
    public void Create_Calls_Modifier()
    {
      var logic = Create();
      var soln = Creator.GetSolution();

      var valres = new ValidationResult();
      _validator.Setup(x => x.Validate(It.IsAny<ValidationContext>())).Returns(valres);

      logic.Create(soln);

      _modifier.Verify(x => x.ForCreate(soln), Times.Once);
    }

    [Test]
    public void Update_Calls_Modifier()
    {
      var logic = Create();
      var soln = Creator.GetSolution();
      var ctx = Creator.GetContext();
      var contact = Creator.GetContact();
      _context.Setup(c => c.HttpContext).Returns(ctx);
      _contacts.Setup(c => c.ByEmail(ctx.Email())).Returns(contact);

      var valres = new ValidationResult();
      _validator.Setup(x => x.Validate(It.IsAny<ValidationContext>())).Returns(valres);

      logic.Update(soln);

      _modifier.Verify(x => x.ForUpdate(soln), Times.Once);
    }

    [Test]
    public void Update_Calls_Notifier_Once()
    {
      var logic = Create();
      var soln = Creator.GetSolution();
      var ctx = Creator.GetContext();
      var contact = Creator.GetContact();
      _context.Setup(c => c.HttpContext).Returns(ctx);
      _contacts.Setup(c => c.ByEmail(ctx.Email())).Returns(contact);

      var valres = new ValidationResult();
      _validator.Setup(x => x.Validate(It.IsAny<ValidationContext>())).Returns(valres);

      logic.Update(soln);

      _notifier.Verify(x => x.Notify(It.IsAny<ChangeRecord<Solutions>>()), Times.Once);
    }

    [Test]
    public void Update_Calls_Notifier_With_Contact()
    {
      var logic = Create();
      var soln = Creator.GetSolution();
      var ctx = Creator.GetContext();
      var contact = Creator.GetContact();
      _context.Setup(c => c.HttpContext).Returns(ctx);
      _contacts.Setup(c => c.ByEmail(ctx.Email())).Returns(contact);

      var valres = new ValidationResult();
      _validator.Setup(x => x.Validate(It.IsAny<ValidationContext>())).Returns(valres);

      logic.Update(soln);

      _notifier.Verify(x => x.Notify(It.Is<ChangeRecord<Solutions>>(cr => cr.ModifierId == contact.Id)), Times.Once);
    }

    [Test]
    public void Update_Calls_Notifier_With_OldEntity()
    {
      var logic = Create();
      var oldSoln = Creator.GetSolution();
      var soln = Creator.GetSolution();
      var ctx = Creator.GetContext();
      var contact = Creator.GetContact();
      _context.Setup(c => c.HttpContext).Returns(ctx);
      _contacts.Setup(c => c.ByEmail(ctx.Email())).Returns(contact);
      _datastore.Setup(ds => ds.ById(soln.Id)).Returns(oldSoln);

      var valres = new ValidationResult();
      _validator.Setup(x => x.Validate(It.IsAny<ValidationContext>())).Returns(valres);

      logic.Update(soln);

      _notifier.Verify(x => x.Notify(It.Is<ChangeRecord<Solutions>>(cr => cr.OldVersion == oldSoln)), Times.Once);
    }

    [Test]
    public void Update_Calls_Notifier_With_NewEntity()
    {
      var logic = Create();
      var soln = Creator.GetSolution();
      var ctx = Creator.GetContext();
      var contact = Creator.GetContact();
      _context.Setup(c => c.HttpContext).Returns(ctx);
      _contacts.Setup(c => c.ByEmail(ctx.Email())).Returns(contact);

      var valres = new ValidationResult();
      _validator.Setup(x => x.Validate(It.IsAny<ValidationContext>())).Returns(valres);

      logic.Update(soln);

      _notifier.Verify(x => x.Notify(It.Is<ChangeRecord<Solutions>>(cr => cr.NewVersion == soln)), Times.Once);
    }

    private SolutionsLogic Create()
    {
      return new SolutionsLogic(_modifier.Object, _datastore.Object, _contacts.Object, _context.Object, _validator.Object, _filter.Object, _evidenceBlobStoreLogic.Object, _notifier.Object);
    }
  }
}

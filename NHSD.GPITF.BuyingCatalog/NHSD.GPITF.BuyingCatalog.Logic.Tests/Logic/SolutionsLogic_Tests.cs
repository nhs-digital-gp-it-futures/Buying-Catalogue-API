using FluentValidation;
using FluentValidation.Internal;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Moq;
using NHSD.GPITF.BuyingCatalog.Interfaces;
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
    private Mock<ISolutionsDatastore> _datastore;
    private Mock<IContactsDatastore> _contacts;
    private Mock<IHttpContextAccessor> _context;
    private Mock<ISolutionsValidator> _validator;
    private Mock<ISolutionsFilter> _filter;

    [SetUp]
    public void SetUp()
    {
      _datastore = new Mock<ISolutionsDatastore>();
      _contacts = new Mock<IContactsDatastore>();
      _context = new Mock<IHttpContextAccessor>();
      _validator = new Mock<ISolutionsValidator>();
      _filter = new Mock<ISolutionsFilter>();
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

    private SolutionsLogic Create()
    {
      return new SolutionsLogic(
        _datastore.Object,
        _contacts.Object,
        _context.Object,
        _validator.Object,
        _filter.Object);
    }
  }
}

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
    private Mock<ISolutionsExDatastore> _datastore;
    private Mock<IContactsDatastore> _contacts;
    private Mock<IHttpContextAccessor> _context;
    private Mock<ISolutionsExValidator> _validator;
    private Mock<ISolutionsExFilter> _filter;

    [SetUp]
    public void SetUp()
    {
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
        _datastore.Object, _context.Object, _validator.Object, _filter.Object,
        _contacts.Object));
    }
  }
}

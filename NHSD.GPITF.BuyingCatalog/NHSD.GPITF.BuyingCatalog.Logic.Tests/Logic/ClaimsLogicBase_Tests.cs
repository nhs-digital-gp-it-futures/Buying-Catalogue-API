using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Moq;
using NHSD.GPITF.BuyingCatalog.Interfaces;
using NHSD.GPITF.BuyingCatalog.Models;
using NHSD.GPITF.BuyingCatalog.Tests;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace NHSD.GPITF.BuyingCatalog.Logic.Tests
{
  [TestFixture]
  public sealed class ClaimsLogicBase_Tests
  {
    private Mock<IHttpContextAccessor> _context;
    private Mock<IClaimsDatastore<ClaimsBase>> _datastore;
    private Mock<IClaimsValidator<ClaimsBase>> _validator;
    private Mock<IClaimsFilter<ClaimsBase>> _filter;

    [SetUp]
    public void SetUp()
    {
      _context = new Mock<IHttpContextAccessor>();
      _datastore = new Mock<IClaimsDatastore<ClaimsBase>>();
      _validator = new Mock<IClaimsValidator<ClaimsBase>>();
      _filter = new Mock<IClaimsFilter<ClaimsBase>>();
    }

    [Test]
    public void Constructor_Completes()
    {
      Assert.DoesNotThrow(() => new DummyClaimsLogicBase(_datastore.Object, _validator.Object, _filter.Object, _context.Object));
    }

    [Test]
    public void ById_CallsFilter()
    {
      var logic = new DummyClaimsLogicBase(_datastore.Object, _validator.Object, _filter.Object, _context.Object);

      logic.ById("some Id");

      _filter.Verify(x => x.Filter(It.IsAny<IEnumerable<ClaimsBase>>()), Times.Once());
    }

    [Test]
    public void BySolution_CallsFilter()
    {
      var logic = new DummyClaimsLogicBase(_datastore.Object, _validator.Object, _filter.Object, _context.Object);

      logic.BySolution("some Id");

      _filter.Verify(x => x.Filter(It.IsAny<IEnumerable<ClaimsBase>>()), Times.Once());
    }
  }
}

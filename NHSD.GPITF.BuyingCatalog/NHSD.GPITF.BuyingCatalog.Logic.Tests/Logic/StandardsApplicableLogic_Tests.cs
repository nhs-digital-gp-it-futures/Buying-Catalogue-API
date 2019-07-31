using Microsoft.AspNetCore.Http;
using Moq;
using NHSD.GPITF.BuyingCatalog.Interfaces;
using NHSD.GPITF.BuyingCatalog.Models;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace NHSD.GPITF.BuyingCatalog.Logic.Tests
{
  [TestFixture]
  public sealed class StandardsApplicableLogic_Tests
  {
    private Mock<IHttpContextAccessor> _context;
    private Mock<IStandardsApplicableDatastore> _datastore;
    private Mock<IStandardsApplicableValidator> _validator;
    private Mock<IStandardsApplicableFilter> _filter;

    [SetUp]
    public void SetUp()
    {
      _context = new Mock<IHttpContextAccessor>();
      _datastore = new Mock<IStandardsApplicableDatastore>();
      _validator = new Mock<IStandardsApplicableValidator>();
      _filter    = new Mock<IStandardsApplicableFilter>();
    }

    [Test]
    public void Constructor_Completes()
    {
      Assert.DoesNotThrow(() => new StandardsApplicableLogic(_datastore.Object, _validator.Object, _filter.Object, _context.Object));
    }


    public static IEnumerable<StandardsApplicableStatus> Statuses()
    {
      return (StandardsApplicableStatus[])Enum.GetValues(typeof(StandardsApplicableStatus));
    }
  }
}

using Moq;
using NHSD.GPITF.BuyingCatalog.Logic.Porcelain;
using NHSD.GPITF.BuyingCatalog.Models;
using NHSD.GPITF.BuyingCatalog.Models.Porcelain;
using NHSD.GPITF.BuyingCatalog.Tests;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace NHSD.GPITF.BuyingCatalog.Logic.Tests.Porcelain
{
  [TestFixture]
  public sealed class SolutionsExFilter_Tests
  {
    private Mock<ISolutionsFilter> _solnFilter;

    [SetUp]
    public void SetUp()
    {
      _solnFilter = new Mock<ISolutionsFilter>();
    }

    [Test]
    public void Constructor_Completes()
    {
      Assert.DoesNotThrow(() => Create());
    }

    [Test]
    public void Filter_Null_DoesNotCallSolutionsFilter()
    {
      var filter = Create();

      // force eval of LINQ
      filter.Filter(new SolutionEx[] { null }).ToList();

      _solnFilter.Verify(x => x.Filter(It.IsAny<IEnumerable<Solutions>>()), Times.Never);
    }

    [Test]
    public void Filter_Solution_CallsSolutionsFilter()
    {
      var filter = Create();
      var soln = Creator.GetSolution();
      var solnEx = Creator.GetSolutionEx(soln: soln);

      // force eval of LINQ
      filter.Filter(new[] { solnEx }).ToList();

      _solnFilter.Verify(x => x.Filter(It.Is<IEnumerable<Solutions>>(solns => solns.All(thisSoln => thisSoln == soln))), Times.Once);
    }

    private SolutionsExFilter Create()
    {
      return new SolutionsExFilter(_solnFilter.Object);
    }
  }
}

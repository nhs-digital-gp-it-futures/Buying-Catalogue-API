using FluentAssertions;
using NHSD.GPITF.BuyingCatalog.Models;
using NHSD.GPITF.BuyingCatalog.Tests;
using NUnit.Framework;
using System.Linq;

namespace NHSD.GPITF.BuyingCatalog.Logic.Tests
{
  [TestFixture]
  public class SolutionsFilter_Tests
  {
    [Test]
    public void Constructor_Completes()
    {
      Assert.DoesNotThrow(() => new SolutionsFilter());
    }

    [Test]
    public void Filter_None_Returns_Approved(
      [ValueSource(typeof(Creator), nameof(Creator.SolutionStatuses))]SolutionStatus status)
    {
      var filter = new SolutionsFilter();
      var solns = new[]
      {
        Creator.GetSolution(status: status),
        Creator.GetSolution(status: status),
        Creator.GetSolution(status: status)
      };
      var expSolns = solns.Where(x => x.Status == SolutionStatus.Approved);

      var res = filter.Filter(solns);

      res.Should().BeEquivalentTo(expSolns);
    }
  }
}

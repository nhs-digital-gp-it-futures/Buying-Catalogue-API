using FluentAssertions;
using NHSD.GPITF.BuyingCatalog.Tests;
using NUnit.Framework;

namespace NHSD.GPITF.BuyingCatalog.Logic.Tests
{
  [TestFixture]
  public sealed class FilterBase_Tests
  {
    [Test]
    public void Constructor_Completes()
    {
      Assert.DoesNotThrow(() => new DummyFilterBase());
    }

    [Test]
    public void Filter_Null_ReturnsEmpty()
    {
      object obj = null;
      var filter = new DummyFilterBase();

      var res = filter.Filter(new[] { obj });

      res.Should().BeEmpty();
    }
  }
}

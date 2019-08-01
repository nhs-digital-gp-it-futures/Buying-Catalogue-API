using FluentAssertions;
using NHSD.GPITF.BuyingCatalog.Tests;
using NUnit.Framework;

namespace NHSD.GPITF.BuyingCatalog.Logic.Tests
{
  [TestFixture]
  public sealed class ContactsFilter_Tests
  {
    [Test]
    public void Constructor_Completes()
    {
      Assert.DoesNotThrow(() => new ContactsFilter());
    }

    [Test]
    public void Filter_None_Returns_All()
    {
      var filter = new ContactsFilter();
      var contacts = new[]
      {
        Creator.GetContact(),
        Creator.GetContact(),
        Creator.GetContact()
      };
      var res = filter.Filter(contacts);

      res.Should().BeEquivalentTo(contacts);
    }
  }
}

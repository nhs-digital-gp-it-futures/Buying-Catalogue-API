using FluentAssertions;
using NHSD.GPITF.BuyingCatalog.Models;
using NHSD.GPITF.BuyingCatalog.Tests;
using NUnit.Framework;
using System;
using System.Linq;

namespace NHSD.GPITF.BuyingCatalog.Logic.Tests
{
  [TestFixture]
  public sealed class OrganisationsFilter_Tests
  {
    [Test]
    public void Constructor_Completes()
    {
      Assert.DoesNotThrow(() => new OrganisationsFilter());
    }

    [Test]
    public void Filter_None_Returns_All()
    {
      var orgFilt = new OrganisationsFilter();
      var govOrg = Creator.GetOrganisation(id: Guid.NewGuid().ToString(), primaryRoleId: PrimaryRole.GovernmentDepartment);
      var supp1Org = Creator.GetOrganisation(id: Guid.NewGuid().ToString(), primaryRoleId: PrimaryRole.ApplicationServiceProvider);
      var supp2Org = Creator.GetOrganisation(id: Guid.NewGuid().ToString(), primaryRoleId: PrimaryRole.ApplicationServiceProvider);
      var orgs = new[] { govOrg, supp1Org, supp2Org };

      var filterOrg = orgFilt.Filter(orgs.ToList());

      filterOrg.Should().BeEquivalentTo(orgs);
    }
  }
}

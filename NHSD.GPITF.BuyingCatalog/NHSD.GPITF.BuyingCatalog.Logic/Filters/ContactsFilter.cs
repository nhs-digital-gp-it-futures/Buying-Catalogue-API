using NHSD.GPITF.BuyingCatalog.Models;

namespace NHSD.GPITF.BuyingCatalog.Logic
{
  public sealed class ContactsFilter : FilterBase<Contacts>, IContactsFilter
  {
    public override Contacts Filter(Contacts input)
    {
      // None
      return input;
    }
  }
}

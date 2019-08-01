using NHSD.GPITF.BuyingCatalog.Logic;
using System;

namespace NHSD.GPITF.BuyingCatalog.Tests
{
  public sealed class DummyFilterBase : FilterBase<object>
  {
    public override object Filter(object input)
    {
      if (input == null)
      {
        throw new ArgumentNullException("Null input should be filtered out in FilterInternal");
      }
      return input;
    }
  }
}

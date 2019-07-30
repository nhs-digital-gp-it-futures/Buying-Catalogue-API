namespace NHSD.GPITF.BuyingCatalog.Interfaces
{
#pragma warning restore CS1591
  // HACK   this is a kludge as this object must know everything else it is affecting.
  //        A better solution would be to post a message to a topic.
  //        Interested parties could then subscribe to the topic.  However, this may
  //        have latency issues regarding message retrieval.  Message TTL would probably
  //        have to be set to twice the (short term) cache expiry.
  public interface IOtherCache
  {
    void ExpireOtherValue(object item);
  }
#pragma warning restore CS1591
}


using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NHSD.GPITF.BuyingCatalog.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace NHSD.GPITF.BuyingCatalog.Logic.Porcelain
{
  public abstract class EqualityComparerBase<T> : IEqualityComparer<T> where T : IHasId
  {
    public bool Equals(T x, T y)
    {
      // remove all DateTime properties due to different handling of UTC in client and server
      var xJobj = RemoveDateTimeProperties(x);
      var yJobj = RemoveDateTimeProperties(y);
      var xJson = JsonConvert.SerializeObject(xJobj);
      var yJson = JsonConvert.SerializeObject(yJobj);

      return xJson == yJson;
    }

    private JObject RemoveDateTimeProperties(T obj)
    {
      var dateProps = obj
        .GetType()
        .GetProperties(BindingFlags.Public | BindingFlags.Instance)
        .Where(prop => prop.PropertyType == typeof(DateTime))
        .ToList();
      var jobj = JObject.FromObject(obj);
      dateProps.ForEach(dt => jobj.Remove(dt.Name));

      return jobj;
    }

    public int GetHashCode(T obj)
    {
      return obj.Id.GetHashCode();
    }
  }
}

using Microsoft.Extensions.Configuration;
using System;
using System.Globalization;

namespace NHSD.GPITF.BuyingCatalog
{
  public static class Settings
  {
    public static string DATASTORE_CONNECTION(IConfiguration config) => Environment.GetEnvironmentVariable("DATASTORE_CONNECTION") ?? config["RepositoryDatabase:Connection"];
    public static string DATASTORE_CONNECTIONTYPE(IConfiguration config, string connection) => Environment.GetEnvironmentVariable("DATASTORE_CONNECTIONTYPE") ?? config[$"RepositoryDatabase:{connection}:Type"];
    public static string DATASTORE_CONNECTIONSTRING(IConfiguration config, string connection) => (Environment.GetEnvironmentVariable("DATASTORE_CONNECTIONSTRING") ?? config[$"RepositoryDatabase:{connection}:ConnectionString"]);

    public static string LOG_CONNECTIONSTRING(IConfiguration config) => Environment.GetEnvironmentVariable("LOG_CONNECTIONSTRING") ?? config["Log:ConnectionString"];
  }
}

using Microsoft.Extensions.Configuration;
using System;
using System.Globalization;

namespace NHSD.GPITF.BuyingCatalog
{
  public static class Settings
  {
    public static string CRM_CLIENTID(IConfiguration config) => Environment.GetEnvironmentVariable("CRM_CLIENTID") ?? config["CRM:ClientId"];
    public static string CRM_CLIENTSECRET(IConfiguration config) => Environment.GetEnvironmentVariable("CRM_CLIENTSECRET") ?? config["CRM:ClientSecret"];
    public static uint CRM_CACHE_EXPIRY_MINS(IConfiguration config) => uint.Parse(Environment.GetEnvironmentVariable("CRM_CACHE_EXPIRY_MINS") ?? config["CRM:CacheExpiryMins"] ?? (7*24*60).ToString(CultureInfo.InvariantCulture));
    public static uint CRM_SHORT_TERM_CACHE_EXPIRY_SECS(IConfiguration config) => uint.Parse(Environment.GetEnvironmentVariable("CRM_SHORT_TERM_CACHE_EXPIRY_SECS") ?? config["CRM:ShortTermCacheExpirySecs"] ?? (10).ToString(CultureInfo.InvariantCulture));

    public static string GIF_CRM_URL(IConfiguration config) => Environment.GetEnvironmentVariable("GIF_CRM_URL") ?? config["CrmUrl"];
    public static string GIF_CRM_AUTHORITY(IConfiguration config) => Environment.GetEnvironmentVariable("GIF_CRM_AUTHORITY") ?? config["CrmAuthority"];
    public static string GIF_AUTHORITY_URI(IConfiguration config) => Environment.GetEnvironmentVariable("GIF_AUTHORITY_URI") ?? config["GIF:Authority_Uri"] ?? "http://localhost:5001";
    public static string GIF_AZURE_CLIENT_ID(IConfiguration config) => Environment.GetEnvironmentVariable("GIF_AZURE_CLIENT_ID") ?? config["AzureClientId"];
    public static string GIF_ENCRYPTED_CLIENT_SECRET(IConfiguration config) => Environment.GetEnvironmentVariable("GIF_ENCRYPTED_CLIENT_SECRET") ?? config["EncryptedClientSecret"];

    public static string DATASTORE_CONNECTION(IConfiguration config) => Environment.GetEnvironmentVariable("DATASTORE_CONNECTION") ?? config["RepositoryDatabase:Connection"];
    public static string DATASTORE_CONNECTIONTYPE(IConfiguration config, string connection) => Environment.GetEnvironmentVariable("DATASTORE_CONNECTIONTYPE") ?? config[$"RepositoryDatabase:{connection}:Type"];
    public static string DATASTORE_CONNECTIONSTRING(IConfiguration config, string connection) => (Environment.GetEnvironmentVariable("DATASTORE_CONNECTIONSTRING") ?? config[$"RepositoryDatabase:{connection}:ConnectionString"]);

    public static string OIDC_ISSUER_URL(IConfiguration config) => Environment.GetEnvironmentVariable("OIDC_ISSUER_URL") ?? config["Jwt:Authority"];
    public static string OIDC_AUDIENCE(IConfiguration config) => Environment.GetEnvironmentVariable("OIDC_AUDIENCE") ?? config["Jwt:Audience"];
    public static string OIDC_USERINFO_URL(IConfiguration config) => Environment.GetEnvironmentVariable("OIDC_USERINFO_URL") ?? config["Jwt:UserInfo"];

    public static bool USE_CRM(IConfiguration config) => bool.Parse(Environment.GetEnvironmentVariable("USE_CRM") ?? config["UseCRM"] ?? false.ToString());

    public static string LOG_CONNECTIONSTRING(IConfiguration config) => Environment.GetEnvironmentVariable("LOG_CONNECTIONSTRING") ?? config["Log:ConnectionString"];
    public static bool LOG_CRM(IConfiguration config) => bool.Parse(Environment.GetEnvironmentVariable("LOG_CRM") ?? config["Log:CRM"] ?? false.ToString());
    public static bool LOG_SHAREPOINT(IConfiguration config) => bool.Parse(Environment.GetEnvironmentVariable("LOG_SHAREPOINT") ?? config["Log:SharePoint"] ?? false.ToString());
    public static bool LOG_BEARERAUTH(IConfiguration config) => bool.Parse(Environment.GetEnvironmentVariable("LOG_BEARERAUTH") ?? config["Log:BearerAuth"] ?? false.ToString());

    public static string CACHE_HOST(IConfiguration config) => Environment.GetEnvironmentVariable("CACHE_HOST") ?? config["Cache:Host"] ?? "localhost";
  }
}

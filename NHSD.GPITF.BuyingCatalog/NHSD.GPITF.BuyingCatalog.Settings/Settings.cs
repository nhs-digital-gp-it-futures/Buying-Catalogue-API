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

    public static string SHAREPOINT_BASEURL(IConfiguration config) => Environment.GetEnvironmentVariable("SHAREPOINT_BASEURL") ?? config["SharePoint:BaseUrl"];
    public static string SHAREPOINT_ORGANISATIONSRELATIVEURL(IConfiguration config) => Environment.GetEnvironmentVariable("SHAREPOINT_ORGANISATIONSRELATIVEURL") ?? config["SharePoint:OrganisationsRelativeUrl"];
    public static string SHAREPOINT_CLIENT_ID(IConfiguration config) => Environment.GetEnvironmentVariable("SHAREPOINT_CLIENT_ID") ?? config["SharePoint:ClientId"];
    public static string SHAREPOINT_CLIENT_SECRET(IConfiguration config) => Environment.GetEnvironmentVariable("SHAREPOINT_CLIENT_SECRET") ?? config["SharePoint:ClientSecret"];
    public static string SHAREPOINT_PROVIDER_ENV(IConfiguration config) => Environment.GetEnvironmentVariable("SHAREPOINT_PROVIDER_ENV");
    public static bool SHAREPOINT_PROVIDER_FAKE(IConfiguration config) => SHAREPOINT_PROVIDER_ENV(config) == "test";
    public static string SHAREPOINT_FILE_DOWNLOAD_SERVER_URL(IConfiguration config) => Environment.GetEnvironmentVariable("SHAREPOINT_FILE_DOWNLOAD_SERVER_URL") ?? config["SharePoint:FileDownloadServerUrl"] ?? "http://localhost:9000/";

    public static string CACHE_HOST(IConfiguration config) => Environment.GetEnvironmentVariable("CACHE_HOST") ?? config["Cache:Host"] ?? "localhost";

    public static bool USE_AMQP(IConfiguration config) => bool.Parse(Environment.GetEnvironmentVariable("USE_AMQP") ?? config["AMQP:UseAMQP"] ?? false.ToString());
    public static bool USE_AZURE_SERVICE_BUS(IConfiguration config) => bool.Parse(Environment.GetEnvironmentVariable("USE_AZURE_SERVICE_BUS") ?? config["AMQP:UseAzureServiceBus"] ?? false.ToString());
    public static string AMQP_PROTOCOL(IConfiguration config) => Environment.GetEnvironmentVariable("AMQP_PROTOCOL") ?? config["AMQP:Protocol"] ?? "amqp";
    public static string AMQP_POLICY_NAME(IConfiguration config) => Environment.GetEnvironmentVariable("AMQP_POLICY_NAME") ?? config["AMQP:PolicyName"] ?? "admin";
    public static string AMQP_POLICY_KEY(IConfiguration config) => Environment.GetEnvironmentVariable("AMQP_POLICY_KEY") ?? config["AMQP:PolicyKey"] ?? "admin";
    public static string AMQP_NAMESPACE_URL(IConfiguration config) => Environment.GetEnvironmentVariable("AMQP_NAMESPACE_URL") ?? config["AMQP:NamespaceUrl"] ?? "localhost:5672";
    public static string AMQP_TOPIC_PREFIX(IConfiguration config) => Environment.GetEnvironmentVariable("AMQP_TOPIC_PREFIX") ?? config["AMQP:TopicPrefix"] ?? "topic://";
    public static uint AMQP_TTL_MINS(IConfiguration config) => uint.Parse(Environment.GetEnvironmentVariable("AMQP_TTL_MINS") ?? config["AMQP:TtlMins"] ?? (7*24*60).ToString(CultureInfo.InvariantCulture));
  }
}

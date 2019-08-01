# Buying Catalogue Public Browse

This is public repository for _Buying Catalogue API_ development during _Public Browse_ stage of GP IT Futures.

## Getting Started

### Development Environment
1. Visual Studio 2017 Community Edition (or higher)
1. .NET Core v2.1 SDK (or higher)
1. database connection:
  * Microsoft SQL Server 2017 (or higher)
  * database creation scripts for other databases are provided
  * databases other than Microsoft SQL Server are not tested and may work (!)

### Environment Variables

There are several environment variables used by the system, mainly for database connection.

Variable | Usage | Example Value
---------|-------|--------------
ASPNETCORE_ENVIRONMENT | ASP.NET Core runtime environment | `Production`, `Development`, `Test`
DATASTORE_CONNECTION | Database connection for datastore | `SqlServer_Cloud`
DATASTORE_CONNECTIONTYPE | Type of database for datastore | `SQLite` or `SqlServer` or `MySql` or `PostgreSql`
DATASTORE_CONNECTIONSTRING | .NET database connection string for database datastore | `Data Source=docker.for.win.localhost;Initial Catalog=BuyingCatalog;User Id=BuyingCatalog;Password=ABCDEFG1234567;`
LOG_CONNECTIONSTRING | .NET database connection string for nLog database target | `Data Source=docker.for.win.localhost;Initial Catalog=BuyingCatalog;User Id=BuyingCatalog;Password=ABCDEFG1234567;`

### Notes
* enable `Development` mode by setting env var:  
&nbsp;&nbsp;&nbsp;&nbsp;  `ASPNETCORE_ENVIRONMENT`=`Development`
* SwaggerUI is only enabled in `Development` mode
* all APIs allow _anonymous_ access


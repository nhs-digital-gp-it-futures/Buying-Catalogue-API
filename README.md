# Buying Catalogue Public Browse

This is public repository for _Buying Catalogue API_ development during _Public Browse_ stage of GP IT Futures.

## Setup

For a brief guide for how to get started developing the API

## Environment Variables

There are several environment variables used by the system, mainly for database connection.

### Buying Catalogue API

Variable | Usage | Example Value
---------|-------|--------------
ASPNETCORE_ENVIRONMENT | ASP.NET Core runtime environment | `Production`, `Development`, `Test`
DATASTORE_CONNECTION | Database connection for datastore | `SqlServer_Cloud`
DATASTORE_CONNECTIONTYPE | Type of database for datastore | `SQLite` or `SqlServer` or `MySql` or `PostgreSql`
DATASTORE_CONNECTIONSTRING | .NET database connection string for database datastore | `Data Source=docker.for.win.localhost;Initial Catalog=BuyingCatalog;User Id=BuyingCatalog;Password=ABCDEFG1234567;`
LOG_CONNECTIONSTRING | .NET database connection string for nLog database target | `Data Source=docker.for.win.localhost;Initial Catalog=BuyingCatalog;User Id=BuyingCatalog;Password=ABCDEFG1234567;`

#### Notes
* enable `Development` mode by setting env var:  
&nbsp;&nbsp;&nbsp;&nbsp;  `ASPNETCORE_ENVIRONMENT`=`Development`
* SwaggerUI is only enabled in `Development` mode
* all APIs allow _anonymous_ access


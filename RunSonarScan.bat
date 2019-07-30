dotnet sonarscanner begin /k:"NHSD:BuyingCatalog"
dotnet build NHSD.GPITF.BuyingCatalog\NHSD.GPITF.BuyingCatalog.sln
dotnet sonarscanner end

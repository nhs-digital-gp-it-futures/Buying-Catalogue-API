using Microsoft.Extensions.Logging;
using NHSD.GPITF.BuyingCatalog.Datastore.Database.Interfaces;
using NHSD.GPITF.BuyingCatalog.Interfaces;
using NHSD.GPITF.BuyingCatalog.Models;
using System.Collections.Generic;

namespace NHSD.GPITF.BuyingCatalog.Datastore.Database
{
  public abstract class EvidenceDatastoreBase<T> : CommonTableExpressionDatastoreBase<T>, IEvidenceDatastore<EvidenceBase> where T : EvidenceBase
  {
    protected EvidenceDatastoreBase(
      IDbConnectionFactory dbConnectionFactory,
      ILogger<EvidenceDatastoreBase<T>> logger,
      ISyncPolicyFactory policy) :
      base(dbConnectionFactory, logger, policy)
    {
    }

    public IEnumerable<IEnumerable<T>> ByClaim(string claimId)
    {
      return ByOwner(claimId);
    }

    protected override string OwnerDiscriminator => nameof(EvidenceBase.ClaimId);

    public override string GetSqlCurrent(string tableName)
    {
      return
 $@"
-- get all previous versions from a specified (CurrentId) version
with recursive Links(CurrentId, {nameof(EvidenceBase.Id)}, {nameof(EvidenceBase.PreviousId)}, {nameof(EvidenceBase.ClaimId)}, {nameof(EvidenceBase.CreatedById)}, {nameof(EvidenceBase.CreatedOn)}, {nameof(EvidenceBase.OriginalDate)}, {nameof(EvidenceBase.Evidence)}, {nameof(EvidenceBase.HasRequestedLiveDemo)}, {nameof(EvidenceBase.BlobId)}) as (
  select
    {nameof(EvidenceBase.Id)}, {nameof(EvidenceBase.Id)}, {nameof(EvidenceBase.PreviousId)}, {nameof(EvidenceBase.ClaimId)}, {nameof(EvidenceBase.CreatedById)}, {nameof(EvidenceBase.CreatedOn)}, {nameof(EvidenceBase.OriginalDate)}, {nameof(EvidenceBase.Evidence)}, {nameof(EvidenceBase.HasRequestedLiveDemo)}, {nameof(EvidenceBase.BlobId)}
  from {tableName}
  where {nameof(EvidenceBase.PreviousId)} is null
  
  union all
  select
    {nameof(EvidenceBase.Id)}, {nameof(EvidenceBase.Id)}, {nameof(EvidenceBase.PreviousId)}, {nameof(EvidenceBase.ClaimId)}, {nameof(EvidenceBase.CreatedById)}, {nameof(EvidenceBase.CreatedOn)}, {nameof(EvidenceBase.OriginalDate)}, {nameof(EvidenceBase.Evidence)}, {nameof(EvidenceBase.HasRequestedLiveDemo)}, {nameof(EvidenceBase.BlobId)}
  from {tableName} 
  where {nameof(EvidenceBase.PreviousId)} is not null
  
  union all
  select
    Links.CurrentId,
    {tableName}.{nameof(EvidenceBase.Id)},
    {tableName}.{nameof(EvidenceBase.PreviousId)},
    {tableName}.{nameof(EvidenceBase.ClaimId)},
    {tableName}.{nameof(EvidenceBase.CreatedById)},
    {tableName}.{nameof(EvidenceBase.CreatedOn)},
    {tableName}.{nameof(EvidenceBase.OriginalDate)},
    {tableName}.{nameof(EvidenceBase.Evidence)},
    {tableName}.{nameof(EvidenceBase.HasRequestedLiveDemo)},
    {tableName}.{nameof(EvidenceBase.BlobId)}
  from Links
  join {tableName}
  on Links.{nameof(EvidenceBase.PreviousId)} = {tableName}.Id
)
  select Links.{nameof(EvidenceBase.Id)}, Links.{nameof(EvidenceBase.PreviousId)}, Links.{nameof(EvidenceBase.ClaimId)}, Links.{nameof(EvidenceBase.CreatedById)}, Links.{nameof(EvidenceBase.CreatedOn)}, Links.{nameof(EvidenceBase.OriginalDate)}, Links.{nameof(EvidenceBase.Evidence)}, Links.{nameof(EvidenceBase.HasRequestedLiveDemo)}, Links.{nameof(EvidenceBase.BlobId)}
  from Links
  where CurrentId = @currentId;
";
    }

    IEnumerable<IEnumerable<EvidenceBase>> IEvidenceDatastore<EvidenceBase>.ByClaim(string claimId)
    {
      return ByClaim(claimId);
    }

    EvidenceBase IEvidenceDatastore<EvidenceBase>.ById(string id)
    {
      return ById(id);
    }

    EvidenceBase IEvidenceDatastore<EvidenceBase>.Create(EvidenceBase evidence)
    {
      return Create((T)evidence);
    }
  }
}

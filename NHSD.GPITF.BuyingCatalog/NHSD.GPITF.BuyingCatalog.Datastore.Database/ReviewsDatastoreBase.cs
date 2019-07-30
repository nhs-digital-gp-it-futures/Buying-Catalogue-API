using Dapper.Contrib.Extensions;
using Microsoft.Extensions.Logging;
using NHSD.GPITF.BuyingCatalog.Datastore.Database.Interfaces;
using NHSD.GPITF.BuyingCatalog.Interfaces;
using NHSD.GPITF.BuyingCatalog.Models;
using System.Collections.Generic;

namespace NHSD.GPITF.BuyingCatalog.Datastore.Database
{
  public abstract class ReviewsDatastoreBase<T> : CommonTableExpressionDatastoreBase<T>, IReviewsDatastore<ReviewsBase> where T : ReviewsBase
  {
    protected ReviewsDatastoreBase(
      IDbConnectionFactory dbConnectionFactory,
      ILogger<ReviewsDatastoreBase<T>> logger,
      ISyncPolicyFactory policy) :
      base(dbConnectionFactory, logger, policy)
    {
    }

    public IEnumerable<IEnumerable<T>> ByEvidence(string evidenceId)
    {
      return ByOwner(evidenceId);
    }

    protected override string OwnerDiscriminator => nameof(ReviewsBase.EvidenceId);

    public override string GetSqlCurrent(string tableName)
    {
      return
$@"
-- get all previous versions from a specified (CurrentId) version
with recursive Links(CurrentId, {nameof(ReviewsBase.Id)}, {nameof(ReviewsBase.PreviousId)}, {nameof(ReviewsBase.EvidenceId)}, {nameof(ReviewsBase.CreatedById)}, {nameof(ReviewsBase.CreatedOn)}, {nameof(ReviewsBase.OriginalDate)}, {nameof(ReviewsBase.Message)}) as (
  select
    {nameof(ReviewsBase.Id)}, {nameof(ReviewsBase.Id)}, {nameof(ReviewsBase.PreviousId)}, {nameof(ReviewsBase.EvidenceId)}, {nameof(ReviewsBase.CreatedById)}, {nameof(ReviewsBase.CreatedOn)}, {nameof(ReviewsBase.OriginalDate)}, {nameof(ReviewsBase.Message)}
  from {tableName}
  where {nameof(ReviewsBase.PreviousId)} is null
  
  union all
  select
    {nameof(ReviewsBase.Id)}, {nameof(ReviewsBase.Id)}, {nameof(ReviewsBase.PreviousId)}, {nameof(ReviewsBase.EvidenceId)}, {nameof(ReviewsBase.CreatedById)}, {nameof(ReviewsBase.CreatedOn)}, {nameof(ReviewsBase.OriginalDate)}, {nameof(ReviewsBase.Message)}
  from {tableName} 
  where {nameof(ReviewsBase.PreviousId)} is not null
  
  union all
  select
    Links.CurrentId,
    {tableName}.{nameof(ReviewsBase.Id)},
    {tableName}.{nameof(ReviewsBase.PreviousId)},
    {tableName}.{nameof(ReviewsBase.EvidenceId)},
    {tableName}.{nameof(ReviewsBase.CreatedById)},
    {tableName}.{nameof(ReviewsBase.CreatedOn)},
    {tableName}.{nameof(ReviewsBase.OriginalDate)},
    {tableName}.{nameof(ReviewsBase.Message)}
  from Links
  join {tableName}
  on Links.{nameof(ReviewsBase.PreviousId)} = {tableName}.Id
)
  select Links.{nameof(ReviewsBase.Id)}, Links.{nameof(ReviewsBase.PreviousId)}, Links.{nameof(ReviewsBase.EvidenceId)}, Links.{nameof(ReviewsBase.CreatedById)}, Links.{nameof(ReviewsBase.CreatedOn)}, Links.{nameof(ReviewsBase.OriginalDate)}, Links.{nameof(ReviewsBase.Message)}
  from Links
  where CurrentId = @currentId;
";
    }

    public void Delete(T review)
    {
      GetInternal(() =>
      {
        using (var trans = _dbConnection.BeginTransaction())
        {
          _dbConnection.Delete(review, trans);
          trans.Commit();

          return 0;
        }
      });
    }

    IEnumerable<IEnumerable<ReviewsBase>> IReviewsDatastore<ReviewsBase>.ByEvidence(string evidenceId)
    {
      return ByEvidence(evidenceId);
    }

    ReviewsBase IReviewsDatastore<ReviewsBase>.ById(string id)
    {
      return ById(id);
    }

    ReviewsBase IReviewsDatastore<ReviewsBase>.Create(ReviewsBase review)
    {
      return Create((T)review);
    }

    void IReviewsDatastore<ReviewsBase>.Delete(ReviewsBase review)
    {
      Delete((T)review);
    }
  }
}

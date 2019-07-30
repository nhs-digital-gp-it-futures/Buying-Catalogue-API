using FluentValidation;
using NHSD.GPITF.BuyingCatalog.Models;

namespace NHSD.GPITF.BuyingCatalog.Logic
{
  public interface IReviewsValidator<in T> : IValidatorBase<T> where T : ReviewsBase
  {
  }
}
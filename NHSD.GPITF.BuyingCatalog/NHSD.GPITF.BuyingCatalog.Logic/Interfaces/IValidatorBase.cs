using FluentValidation;

namespace NHSD.GPITF.BuyingCatalog.Logic
{
  public interface IValidatorBase<in T> : IValidator<T>
  {
    void ValidateAndThrowEx(T instance, string ruleSet = null);
  }
}

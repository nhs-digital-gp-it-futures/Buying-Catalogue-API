﻿using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Linq;

namespace NHSD.GPITF.BuyingCatalog
{
  /// <summary>
  /// Represents the Swagger/Swashbuckle operation filter used to document the implicit API version parameter.
  /// </summary>
  /// <remarks>This <see cref="IOperationFilter"/> is only required due to bugs in the <see cref="SwaggerGenerator"/>.
  /// Once they are fixed and published, this class can be removed.</remarks>
  public sealed class SwaggerDefaultValues : IOperationFilter
  {
    /// <summary>
    /// Applies the filter to the specified operation using the given context.
    /// </summary>
    /// <param name="operation">The operation to apply the filter to.</param>
    /// <param name="context">The current operation filter context.</param>
    public void Apply(Operation operation, OperationFilterContext context)
    {
      var apiDescription = context.ApiDescription;

      if (operation.Parameters == null)
      {
        return;
      }

      // REF: https://github.com/domaindrivendev/Swashbuckle.AspNetCore/issues/412
      // REF: https://github.com/domaindrivendev/Swashbuckle.AspNetCore/pull/413
      foreach (var parameter in operation.Parameters.OfType<NonBodyParameter>())
      {
        var description = apiDescription.ParameterDescriptions.First(p => p.Name == parameter.Name);

        if (parameter.Description == null)
        {
          parameter.Description = description.ModelMetadata?.Description;
        }

        // START_HACK   look for API version parameter in Swagger generated UI
        if (parameter.Default == null &&
          parameter.Name == "version")
        {
          parameter.Default = DefaultValues.ApiVersion.MajorVersion;
        }
        // END_HACK
      }
    }
  }
}

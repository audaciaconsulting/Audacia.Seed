using System.Data.Entity.Validation;
using Audacia.Seed.Extensions;

namespace Audacia.Seed.EntityFramework.Extensions;

/// <summary>
/// Extension methods for <see cref="Exception"/>s.
/// </summary>
public static class ExceptionExtensions
{
    /// <summary>
    /// Extract meaningful information from the provided exception.
    /// </summary>
    /// <param name="exception">The exception containing the information.</param>
    /// <returns>A more developer-friendly message from the exception.</returns>
    public static string GetErrors(this DbEntityValidationException exception)
    {
        ArgumentNullException.ThrowIfNull(exception);

        var validationErrors = exception.EntityValidationErrors
            .SelectMany(eve => eve.ValidationErrors.Select(ve =>
                $"{eve.Entry.Entity.GetType().GetFormattedName()} | {ve.PropertyName} | {ve.ErrorMessage}"));

        return $"{exception.Message}{Environment.NewLine}{string.Join(". ", validationErrors)}";
    }
}
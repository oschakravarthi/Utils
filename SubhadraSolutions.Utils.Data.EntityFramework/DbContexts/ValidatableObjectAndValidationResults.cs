using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.ComponentModel.DataAnnotations;

namespace SubhadraSolutions.Utils.Data.EntityFramework.DbContexts;

public class ValidatableObjectAndValidationResults
{
    public ValidatableObjectAndValidationResults(EntityEntry<IValidatableObject> entry, ValidationResult[] validationErrors)
    {
        this.Entry = entry;
        this.ValidationErrors = validationErrors;
    }

    public EntityEntry<IValidatableObject> Entry { get; private set; }
    public ValidationResult[] ValidationErrors { get; private set; }
}
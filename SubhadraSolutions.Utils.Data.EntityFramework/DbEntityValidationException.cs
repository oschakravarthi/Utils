using SubhadraSolutions.Utils.Data.EntityFramework.DbContexts;
using System;

namespace SubhadraSolutions.Utils.Data.EntityFramework;

public class DbEntityValidationException : Exception
{
    public DbEntityValidationException(ValidatableObjectAndValidationResults[] entityValidationErrors)
    {
        this.EntityValidationErrors = entityValidationErrors;
    }

    public DbEntityValidationException(ValidatableObjectAndValidationResults[] entityValidationErrors, string message) : base(message)
    {
        this.EntityValidationErrors = entityValidationErrors;
    }

    public DbEntityValidationException(ValidatableObjectAndValidationResults[] entityValidationErrors, string message, Exception innerException) : base(message, innerException)
    {
        this.EntityValidationErrors = entityValidationErrors;
    }

    public ValidatableObjectAndValidationResults[] EntityValidationErrors { get; private set; }
}
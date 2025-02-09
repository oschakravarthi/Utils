using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace SubhadraSolutions.Utils.Data.EntityFramework.DbContexts;

public class AbstractDbContext : DbContext
{
    public AbstractDbContext(DbContextOptions options) : base(options)
    {
    }

    protected AbstractDbContext()
    {
    }

    public override int SaveChanges()
    {
        var result = new List<ValidatableObjectAndValidationResults>();
        foreach (var entry in ChangeTracker
                     .Entries<IValidatableObject>())
        {
            var validationResults = entry.Entity.Validate(null).ToArray();
            if (validationResults.Length > 0)
            {
                result.Add(new ValidatableObjectAndValidationResults(entry, validationResults));
            }
        }
        if (result.Count > 0)
        {
            throw new DbEntityValidationException(result.ToArray());
        }

        return base.SaveChanges();
    }
}
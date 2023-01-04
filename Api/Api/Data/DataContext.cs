using Api.Data.Entities;
using Api.Data.Extensions;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Api.Data;

public class DataContext : IdentityDbContext<AppUser, AppRole, Guid>
{
    public DataContext(DbContextOptions options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Some tablenames and indexes are not converted to snake case. Convert them here.
        foreach (var entity in modelBuilder.Model.GetEntityTypes())
        {
            var tableName = modelBuilder.Entity(entity.Name).Metadata.GetTableName();
            if (tableName != null)
            {
                var newTableName = tableName.ToSnakeCase();
                modelBuilder.Entity(entity.Name).Metadata.SetTableName(newTableName);
                foreach (var index in entity.GetIndexes())
                {
                    var columns = index.Properties.Select(p => p.GetColumnName());
                    var combinedColumns = String.Join("_", columns);
                    index.SetDatabaseName($"ix_{newTableName}_{combinedColumns}");
                }
            }
        }
    }
}
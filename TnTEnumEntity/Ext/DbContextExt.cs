using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using Microsoft.EntityFrameworkCore;

namespace TnTEnumEntity.Ext;

/// <summary>
///     Extension methods for <see cref="DbContext" /> to handle enum entities.
/// </summary>
public static class DbContextExt {

    /// <summary>
    ///     Ensures that entities implementing <see cref="IEnumEntity" /> are not modified or deleted.
    /// </summary>
    /// <param name="dbContext">The <see cref="DbContext" /> instance.</param>
    /// <exception cref="InvalidOperationException">Thrown when an attempt is made to modify or delete an enum entity.</exception>
    public static void EnsureCorrectHandlingOfEnumEntities(this DbContext dbContext) {
        foreach (var entity in dbContext.ChangeTracker.Entries().Where(e => e.Entity is IEnumEntity)) {
            entity.State = entity.State switch {
                EntityState.Modified => throw new InvalidOperationException($"Modifying any entity of type {nameof(IEnumEntity)} is not allowed."),
                EntityState.Deleted => throw new InvalidOperationException($"Deleting any entity of type {nameof(IEnumEntity)} is not allowed."),
                _ => EntityState.Unchanged, // In the event of addition or other allowed state, we don't actually want to do anything.
            };
        }
    }

    /// <summary>
    ///     Seeds the enum entities into the model.
    /// </summary>
    /// <param name="builder">The <see cref="ModelBuilder" /> instance.</param>
    public static void SeedEnumEntities(this ModelBuilder builder) {
        var type = typeof(IEnumEntity);
        var enumEntities = Assembly.GetCallingAssembly()
            ?.GetTypes()
            .Where(t => type.IsAssignableFrom(t) && t.IsClass && !t.IsAbstract);

        foreach (var enumEntity in enumEntities ?? []) {
            var method = enumEntity.GetMethod("GetValues", BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);
            if (method is not null) {
                builder.Entity(enumEntity).HasData(method.Invoke(null, null) as IEnumerable<object> ?? []);
            }
        }
    }
}
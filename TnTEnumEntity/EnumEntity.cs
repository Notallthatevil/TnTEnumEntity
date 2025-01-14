using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Collections.Immutable;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;
using System.Linq;

namespace TnTEnumEntity;

/// <summary>
///     Abstract base class for enum entities.
/// </summary>
/// <typeparam name="TEnum">The type of the enum.</typeparam>
/// <typeparam name="TDerived">The type of the derived class.</typeparam>
public abstract class EnumEntity<TEnum, TDerived> : IEnumEntity where TEnum : struct, Enum where TDerived : EnumEntity<TEnum, TDerived>, new() {

    /// <summary>
    ///     Gets the type of the enum.
    /// </summary>
    [NotMapped]
    public static Type Type => typeof(TEnum);

    /// <summary>
    ///     Gets or sets the description of the enum value.
    /// </summary>
    [StringLength(255)]
    public string? Description { get; set; }

    /// <summary>
    ///     Gets or sets the name of the enum value.
    /// </summary>
    [Required, StringLength(255)]
    public string Name { get => Value.ToString(); set { } }

    /// <summary>
    ///     Gets or sets the enum value.
    /// </summary>
    [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
    public TEnum Value { get; set; }

    private static readonly ImmutableDictionary<TEnum, TDerived> _values = Enum.GetValues(typeof(TEnum)).Cast<TEnum>().ToImmutableDictionary(static e => e, e => new TDerived { Value = e, Name = e.ToString(), Description = GetDescriptionAttribute(e) });

    /// <summary>
    ///     Indexer to get the derived entity for the given enum value.
    /// </summary>
    /// <param name="enum">The enum value.</param>
    /// <returns>The derived entity.</returns>
    public TDerived this[TEnum @enum] => ToDerived(@enum);

    /// <summary>
    ///     Gets all the values of the enum as derived entities.
    /// </summary>
    /// <returns>An enumerable of derived entities.</returns>
    public static IEnumerable<object> GetValues() => _values.Values;

    /// <summary>
    ///     Implicit conversion from enum value to enum entity.
    /// </summary>
    /// <param name="value">The enum value.</param>
    public static implicit operator EnumEntity<TEnum, TDerived>(TEnum value) {
        return new TDerived {
            Value = value,
            Name = value.ToString(),
            Description = GetDescriptionAttribute(value)
        };
    }

    /// <summary>
    ///     Implicit conversion from enum entity to enum value.
    /// </summary>
    /// <param name="entity">The enum entity.</param>
    public static implicit operator TEnum(EnumEntity<TEnum, TDerived> entity) => (TEnum)Enum.ToObject(typeof(TEnum), entity?.Value ?? default);

    /// <summary>
    ///     Converts the enum value to the derived entity.
    /// </summary>
    /// <param name="enum">The enum value.</param>
    /// <returns>The derived entity.</returns>
    public static TDerived ToDerived(TEnum @enum) => _values[@enum];

    /// <summary>
    ///     Determines whether the specified object is equal to the current object.
    /// </summary>
    /// <param name="obj">The object to compare with the current object.</param>
    /// <returns>true if the specified object is equal to the current object; otherwise, false.</returns>
    public override bool Equals(object? obj) => obj is EnumEntity<TEnum, TDerived> entity && obj is not null && Value.Equals(entity.Value);

    /// <summary>
    ///     Serves as the default hash function.
    /// </summary>
    /// <returns>A hash code for the current object.</returns>
    public override int GetHashCode() => Value.GetHashCode();

    /// <summary>
    ///     Gets the description attribute of the enum value.
    /// </summary>
    /// <param name="enum">The enum value.</param>
    /// <returns>The description attribute.</returns>
    private static string GetDescriptionAttribute(TEnum @enum) => @enum.GetType()
        .GetField(@enum.ToString())
        ?.GetCustomAttributes(typeof(DescriptionAttribute), false)
        .Cast<DescriptionAttribute>()
        .FirstOrDefault()?.Description ?? string.Empty;
}

/// <summary>
///     Interface for enum entities.
/// </summary>
public interface IEnumEntity {
#pragma warning disable CA2252

    /// <summary>
    ///     Gets or sets the description of the enum value.
    /// </summary>
    string? Description { get; set; }

    /// <summary>
    ///     Gets or sets the name of the enum value.
    /// </summary>
    string Name { get; set; }

#if NET8_0_OR_GREATER
    /// <summary>
    ///     Gets the type of the enum.
    /// </summary>
    static abstract Type Type { get; }

    /// <summary>
    ///     Gets all the values of the enum as derived entities.
    /// </summary>
    /// <returns>An enumerable of derived entities.</returns>
    static abstract IEnumerable<object> GetValues();
#endif

#pragma warning restore CA2252
}
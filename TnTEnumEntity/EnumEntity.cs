using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Collections.Immutable;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace TnTEnumEntity;

public abstract class EnumEntity<TEnum, TDerived> : IEnumEntity where TEnum : struct, Enum where TDerived : EnumEntity<TEnum, TDerived>, new() {

    [NotMapped]
    public static Type Type => typeof(TEnum);

    [StringLength(255)]
    public string? Description { get; set; }

    [Required, StringLength(255)]
    public string Name { get; set; } = default!;

    [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
    public TEnum Value { get; set; }

    private readonly static ImmutableArray<TDerived> _values = Enum.GetValues<TEnum>().Select(e => new TDerived { Value = e, Name = e.ToString(), Description = GetDescriptionAttribute(e) }).ToImmutableArray();

    public TDerived this[TEnum @enum] => ToDerived(@enum);

    public static TDerived ToDerived(TEnum @enum) => _values.First(e => e.Value.Equals(@enum));

    public static IEnumerable<object> GetValues() {
        return Enum.GetValues<TEnum>().Select(e => new TDerived { Value = e, Name = e.ToString(), Description = GetDescriptionAttribute(e) });
    }

    public static implicit operator EnumEntity<TEnum, TDerived>(TEnum value) {
        return new TDerived {
            Value = value,
            Name = value.ToString(),
            Description = GetDescriptionAttribute(value)
        };
    }

    public static implicit operator TEnum(EnumEntity<TEnum, TDerived> entity) {
        return (TEnum)Enum.ToObject(typeof(TEnum), entity?.Value ?? default);
    }

    public override bool Equals(object? obj) {
        if (obj is EnumEntity<TEnum, TDerived> entity && obj is not null) {
            return Value.Equals(entity.Value);
        }
        return false;
    }

    public override int GetHashCode() {
        return Value.GetHashCode();
    }

    private static string GetDescriptionAttribute(TEnum @enum) => @enum.GetType()
        .GetField(@enum.ToString())
        ?.GetCustomAttributes(typeof(DescriptionAttribute), false)
        .Cast<DescriptionAttribute>()
        .FirstOrDefault()?.Description ?? string.Empty;
}

public interface IEnumEntity {
#pragma warning disable CA2252
    static abstract Type Type { get; }
    string? Description { get; set; }
    string Name { get; set; }

    static abstract IEnumerable<object> GetValues();

#pragma warning restore CA2252
}
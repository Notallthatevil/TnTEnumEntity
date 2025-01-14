using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Xunit;

namespace TnTEnumEntity.Tests {
    public enum SampleEnum {
        [Description("First Value")]
        First,
        [Description("Second Value")]
        Second,
        Third
    }

    public class SampleEnumEntity : EnumEntity<SampleEnum, SampleEnumEntity> { }

    public class EnumEntityTests {
        [Fact]
        public void Type_ShouldReturnEnumType() {
            Assert.Equal(typeof(SampleEnum), SampleEnumEntity.Type);
        }

        [Fact]
        public void Description_ShouldReturnCorrectDescription() {
            var entity = new SampleEnumEntity()[SampleEnum.First];
            Assert.Equal("First Value", entity.Description);
        }

        [Fact]
        public void Name_ShouldReturnEnumName() {
            var entity = new SampleEnumEntity { Value = SampleEnum.Second };
            Assert.Equal("Second", entity.Name);
        }

        [Fact]
        public void Value_ShouldSetAndGetEnumValue() {
            var entity = new SampleEnumEntity { Value = SampleEnum.Third };
            Assert.Equal(SampleEnum.Third, entity.Value);
        }

        [Fact]
        public void Indexer_ShouldReturnCorrectEntity() {
            var entity = new SampleEnumEntity();
            var derivedEntity = entity[SampleEnum.First];
            Assert.Equal(SampleEnum.First, derivedEntity.Value);
        }

        [Fact]
        public void GetValues_ShouldReturnAllEnumEntities() {
            var values = SampleEnumEntity.GetValues().Cast<SampleEnumEntity>().ToList();
            Assert.Equal(3, values.Count);
            Assert.Contains(values, v => v.Value == SampleEnum.First);
            Assert.Contains(values, v => v.Value == SampleEnum.Second);
            Assert.Contains(values, v => v.Value == SampleEnum.Third);
        }

        [Fact]
        public void ImplicitConversion_FromEnumToEntity_ShouldReturnCorrectEntity() {
            SampleEnumEntity entity = new SampleEnumEntity()[SampleEnum.First];
            Assert.Equal(SampleEnum.First, entity.Value);
        }

        [Fact]
        public void ImplicitConversion_FromEntityToEnum_ShouldReturnCorrectEnum() {
            var entity = new SampleEnumEntity { Value = SampleEnum.Second };
            SampleEnum value = entity;
            Assert.Equal(SampleEnum.Second, value);
        }

        [Fact]
        public void Equals_ShouldReturnTrueForEqualEntities() {
            var entity1 = new SampleEnumEntity { Value = SampleEnum.Third };
            var entity2 = new SampleEnumEntity { Value = SampleEnum.Third };
            Assert.True(entity1.Equals(entity2));
        }

        [Fact]
        public void GetHashCode_ShouldReturnEnumValueHashCode() {
            var entity = new SampleEnumEntity { Value = SampleEnum.First };
            Assert.Equal(SampleEnum.First.GetHashCode(), entity.GetHashCode());
        }
    }
}

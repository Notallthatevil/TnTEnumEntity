# TnTEnumEntity

## Overview

TnTEnumEntity is a library designed to facilitate the handling of enum entities in Entity Framework Core. It provides an abstract base class for enum entities and extension methods for `DbContext` to ensure correct handling and seeding of enum entities.

## Features

- Abstract base class for enum entities.
- Extension methods for `DbContext` to handle enum entities.
- Ensures enum entities are not modified or deleted.
- Seeds enum entities into the model.

## Installation

To install TnTEnumEntity, add the following package to your project:

```bash
dotnet add package TnTEnumEntity
```

## Usage

### EnumEntity Base Class

The `EnumEntity` base class provides a structure for defining enum entities. It includes properties for the enum value, name, and description, as well as methods for converting between enum values and derived entities.

#### Example

```csharp
public enum Status { 
    [Description("Active status")] 
    Active, 
    [Description("Inactive status")] 
    Inactive 
}

public class StatusEntity : EnumEntity<Status, StatusEntity> { }
```


### DbContext Extensions

The `DbContextExt` class provides extension methods for `DbContext` to handle enum entities.

#### EnsureCorrectHandlingOfEnumEntities

This method ensures that entities implementing `IEnumEntity` are not modified or deleted.

```csharp
public class MyDbContext : DbContext { 
    public override int SaveChanges() { 
        this.EnsureCorrectHandlingOfEnumEntities(); 
        return base.SaveChanges(); 
    } 
}
```


#### SeedEnumEntities

This method seeds the enum entities into the model.

```csharp
protected override void OnModelCreating(ModelBuilder modelBuilder) { 
    modelBuilder.SeedEnumEntities(); 
    base.OnModelCreating(modelBuilder); 
}
```


## License
This project is licensed under the MIT License.

using Microsoft.EntityFrameworkCore;

namespace Data.Entities;

/// <summary>
/// Indicates that implementing class is an Entity Framework entity.
/// All entities stored with EF are recommended to implement this type to ensure they are properly configured.
/// </summary>
/// <typeparam name="TSelf">The implementing type.</typeparam>
/// <typeparam name="TConfiguration">
/// The <see cref="IEntityTypeConfiguration{TEntity}"/>
/// used to configure <typeparamref name="TSelf"/>
/// </typeparam>
public interface IEntity<TSelf, TConfiguration>
    where TSelf : class, IEntity<TSelf, TConfiguration>
    where TConfiguration : IEntityTypeConfiguration<TSelf>;
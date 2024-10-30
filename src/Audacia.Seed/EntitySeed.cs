using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using Audacia.Seed.Contracts;
using Audacia.Seed.Customisation;
using Audacia.Seed.Exceptions;
using Audacia.Seed.Extensions;
using Audacia.Seed.Helpers;
using Audacia.Seed.Models;
using Audacia.Seed.Options;
using Audacia.Seed.Properties;

namespace Audacia.Seed;

/// <summary>
/// Base class for seeding a specific entity to the database, with utility methods to facilitate customisability.
/// </summary>
/// <typeparam name="TEntity">The type of the entity being seeded.</typeparam>
public class EntitySeed<TEntity> : IEntitySeed<TEntity>
    where TEntity : class
{
    /// <inheritdoc />
    public ISeedableRepository? Repository { get; set; }

    /// <summary>
    /// Gets a list of how we'll override the default behaviour of seeding specific properties on this entity.
    /// </summary>
    public List<ISeedCustomisation<TEntity>> Customisations { get; } = [];

    /// <summary>
    /// Gets a value indicating whether the default behaviour of this seed has been overridden.
    /// </summary>
    public bool HasCustomisations => Customisations.Any();

    /// <summary>
    /// Gets the <see cref="Type"/> of the entity this seed is for.
    /// </summary>
    public Type EntityType => typeof(TEntity);

    /// <inheritdoc />
    public EntitySeedOptions Options { get; } = new();

    /// <summary>
    /// Get a list of prerequisites that this entity depends on e.g a required parent.
    /// </summary>
    /// <returns>A list of prerequisites that will be seeded before this.</returns>
    public virtual IEnumerable<ISeedPrerequisite> Prerequisites()
    {
        var modelInformation = Repository!.GetEntityModelInformation<TEntity>();
        if (!modelInformation.RequiredNavigationProperties.Any())
        {
            return [];
        }

        var prerequisites = GetSeedPrerequisites(modelInformation);

        return prerequisites;
    }

    private static List<ISeedPrerequisite> GetSeedPrerequisites(IEntityModelInformation modelInformation)
    {
        var assembly = EntryPointAssembly.Load();
        List<ISeedPrerequisite> prerequisites = [];
        foreach (var requiredNavigationProperty in modelInformation.RequiredNavigationProperties)
        {
            var seedPrerequisite = GetSeedPrerequisite(assembly, requiredNavigationProperty, modelInformation);
            prerequisites.Add(seedPrerequisite);
        }

        return prerequisites;
    }

    private static ISeedPrerequisite GetSeedPrerequisite(
        Assembly assembly,
        NavigationPropertyConfiguration requiredNavigationProperty,
        IEntityModelInformation modelInformation)
    {
        var prerequisiteSeed = assembly.FindSeed(requiredNavigationProperty.NavigationProperty.PropertyType);
        var par = Expression.Parameter(typeof(TEntity), "x");

        var lambda = Expression.Lambda(
            Expression.Property(par, requiredNavigationProperty.NavigationProperty.Name),
            par);

        var prerequisiteSeedType =
            typeof(SeedPrerequisite<,>).MakeGenericType(
                typeof(TEntity),
                requiredNavigationProperty.NavigationProperty.PropertyType);
        var seedPrerequisite = (ISeedPrerequisite)Activator.CreateInstance(
            prerequisiteSeedType,
            lambda,
            prerequisiteSeed)!;

        // Override the default behaviour to re-use existing entities if the navigation property is a primary key.
        if (modelInformation.PrimaryKey?.Any(p => p == requiredNavigationProperty.ForeignKeyProperty) == true)
        {
            seedPrerequisite.Seed.Options.InsertionBehavior = SeedingInsertionBehaviour.AddNew;
        }

        return seedPrerequisite;
    }

    /// <summary>
    /// Prepare this to be added to the <paramref name="repository"/>.
    /// </summary>
    /// <param name="repository">The repository to seed into.</param>
    public void PrepareToSeed(ISeedableRepository repository)
    {
        Repository = repository;
        if (Options.InsertionBehavior != SeedingInsertionBehaviour.TryFindExisting)
        {
            Options.InsertionBehavior = SeedingInsertionBehaviour.AddNew;
        }
    }

    /// <inheritdoc />
    public void PerformSeeding(ISeedableRepository seedableRepository)
    {
        ArgumentNullException.ThrowIfNull(seedableRepository);

        Repository = seedableRepository;

        if (Options.AmountToCreate > 1)
        {
            _ = BuildMany(Options.AmountToCreate).ToList();
        }
        else
        {
            Build();
        }
    }

    /// <summary>
    /// Get a default <typeparamref name="TEntity"/> in its simplest form before prerequisites are populated.
    /// </summary>
    /// <param name="index">The index of seeding this specific entity. Use if seeding more than one and you want behaviour to change based on this index.</param>
    /// <param name="previous">The previously created entity at the index before this. Use if seeding more than one, and you want to re-use properties on the previous entity.</param>
    /// <returns>An entity in its simplest form without prerequisites populated.</returns>
    /// <exception cref="DataSeedingException">If we cannot find an appropriate constructor.</exception>
    protected virtual TEntity GetDefault(int index, TEntity? previous)
    {
        var type = typeof(TEntity);
        var simplestConstructor = type.GetConstructors().MinBy(c => c.GetParameters().Length)
                                  ?? throw new DataSeedingException($"Cannot find a Constructor for {type.Name}");
        var constructorParameters = simplestConstructor.GetParameters();

        var valuesForConstructor = constructorParameters
            .Select(p => p.ParameterType.ExampleValue())
            .ToArray();

        var instance = (TEntity)simplestConstructor.Invoke(valuesForConstructor);

        return instance;
    }

    /// <summary>
    /// Get a predicate that would match this seed to a <typeparamref name="TEntity"/> already in the repository.
    /// </summary>
    /// <param name="index">The index of the entity being seeded.</param>
    /// <returns>A predicate used to match this seed to an entity.</returns>
    public virtual Expression<Func<TEntity, bool>> Predicate(int index)
    {
        return _ => true;
    }

    /// <summary>
    /// Get a pre-existing entity from the database.
    /// </summary>
    /// <typeparam name="TNavigation">The type of the navigation property to set.</typeparam>
    /// <returns>An entity returned from the database.</returns>
    /// <exception cref="DataSeedingException">If no existing entities could be found matching the predicate.</exception>
    protected TNavigation FromDb<TNavigation>()
        where TNavigation : class
    {
        Expression<Func<TNavigation, bool>> predicate = _ => true;
        return FromDb(predicate);
    }

    /// <summary>
    /// Get a pre-existing entity from the database.
    /// </summary>
    /// <param name="predicate">The predicate to match the entity.</param>
    /// <typeparam name="TNavigation">The type of the navigation property to set.</typeparam>
    /// <returns>An entity returned from the database.</returns>
    /// <exception cref="DataSeedingException">If no existing entities could be found matching the predicate.</exception>
    protected virtual TNavigation FromDb<TNavigation>(Expression<Func<TNavigation, bool>> predicate)
        where TNavigation : class
    {
        var query = Repository?.DbSet<TNavigation>() ??
                    throw new DataSeedingException($"Unable to seed data as the repository is null. Make sure you are calling this from the {nameof(GetDefault)} method.");

        var matchedEntities = query
            .Where(predicate)
            .ToList();
        if (!matchedEntities.Any())
        {
            var sb = new StringBuilder($"You must set up at least one {typeof(TNavigation)} ");

            sb.Append(
                "in the database. ");
            sb.Append("Look further down this stack trace to see which SeedConfiguration class is being run, ");
            sb.Append("and ensure that it already exists in the database.");

            throw new DataSeedingException(sb.ToString());
        }

        return matchedEntities.First();
    }

    /// <summary>
    /// Build the entities that are to be added to the database.
    /// </summary>
    /// <param name="amountToCreate">The amount of entities to create.</param>
    /// <returns>An enumerable with count <paramref name="amountToCreate"/> of valid entities to seed.</returns>
    internal IEnumerable<TEntity> BuildMany(int amountToCreate)
    {
        Options.AmountToCreate = amountToCreate;
        if (Options.InsertionBehavior != SeedingInsertionBehaviour.TryFindExisting)
        {
            Options.InsertionBehavior = amountToCreate > 1
                ? SeedingInsertionBehaviour.AddNew
                : SeedingInsertionBehaviour.TryFindNew;
        }

        TEntity? previous = null;
        for (var index = 0; index < Options.AmountToCreate; index++)
        {
            var entity = GetOrCreateEntity(index, previous);

            previous = entity;
            yield return entity;
        }
    }

    /// <summary>
    /// Build the entity that is to be added to the database.
    /// </summary>
    /// <returns>A valid entity to seed.</returns>
    public TEntity Build()
    {
        var entity = GetOrCreateEntity(0, null);
        return entity;
    }

    /// <summary>
    /// Custom implementation so that equality operations only care about the type.
    /// </summary>
    /// <param name="obj">The other object to compare to.</param>
    /// <returns>Whether this object equals the <paramref name="obj"/>.</returns>
    public override bool Equals(object? obj)
    {
        return obj != null && obj.GetType() == GetType();
    }

    /// <summary>
    /// Custom implementation so that equality operations only care about the type.
    /// </summary>
    /// <returns>The hashcode unique to this type.</returns>
    public override int GetHashCode()
    {
        return GetType().GetHashCode();
    }

    /// <summary>
    /// Add a customisation to the entity seed.
    /// </summary>
    /// <param name="customisation">The customisation to add.</param>
    public void AddCustomisation(ISeedCustomisation<TEntity> customisation)
    {
        var matchingCustomisation = Customisations.Find(c => c.Equals(customisation));
        if (matchingCustomisation != null)
        {
            matchingCustomisation.Merge(customisation);
        }
        else
        {
            Customisations.Add(customisation);
        }
    }

    /// <summary>
    /// Get a <typeparamref name="TEntity"/> from the repository, or create a new one if it doesn't exist.
    /// </summary>
    /// <param name="index">The current index. This will be greater than zero if creating many.</param>
    /// <param name="previous">The entity seeded previously. This will not be null if creating many.</param>
    /// <returns>A new or existing entity depending on whether we found an existing appropriate one.</returns>
    /// <exception cref="DataSeedingException">If we don't have a <see cref="Repository"/> set.</exception>
    internal TEntity GetOrCreateEntity(int index, TEntity? previous)
    {
        if (Repository == null)
        {
            throw new DataSeedingException("Cannot build entities as there is no repository set.");
        }

        var predicate = this.ToPredicate(index);
        var entity = Options.InsertionBehavior switch
        {
            SeedingInsertionBehaviour.TryFindExisting => Repository.FindLocal(predicate)
                                                          ?? Repository.DbSet<TEntity>().FirstOrDefault(predicate),
            SeedingInsertionBehaviour.TryFindNew => Repository.FindLocal(predicate),
            _ => null
        };

        // If we didn't find an existing entity, we need to create a new one.
        if (entity == null)
        {
            entity = GetDefault(index, previous);

            PopulatePrerequisites(entity);

            // The entity is considered valid at this point, so we can add it to the repository.
            Repository.Add(entity);

            PopulateCustomisations(index, previous, entity);
        }

        return entity;
    }

    private void PopulateCustomisations(int index, TEntity? previous, TEntity entity)
    {
        var uniqueCustomisations = Customisations.OrderBy(c => c.Order).Distinct().ToList();
        foreach (var customisation in uniqueCustomisations)
        {
            customisation.Validate(this);
            customisation.Apply(entity, Repository!, index, previous);
        }
    }

    private void PopulatePrerequisites(TEntity entity)
    {
        var seedPrerequisites = Prerequisites().ToList();
        foreach (var seedPrerequisite in seedPrerequisites
                     .Where(sp => Customisations.All(c => !c.EqualsPrerequisite(sp))))
        {
            var seed = seedPrerequisite.Seed;
            seed.GetType().GetProperty(nameof(Repository))?.SetValue(seed, Repository);
            var buildMethod = seed.GetType().GetMethod(nameof(Build), BindingFlags.Instance | BindingFlags.Public);
            var navigationProperty = buildMethod?.Invoke(seed, null)!;
            seedPrerequisite.PropertyInfo.SetValue(entity, navigationProperty);

            if (seed.Options.InsertionBehavior != SeedingInsertionBehaviour.TryFindExisting)
            {
                var addMethod = Repository!
                    .GetType()
                    .GetMethod(nameof(Repository.Add))!
                    .MakeGenericMethod(navigationProperty.GetType());
                addMethod.Invoke(Repository, [navigationProperty]);
            }
        }
    }
}
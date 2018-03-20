# DynamicLinqRepository

Repository project that uses Entity Framework (>=6.2.0) and System.Linq.Dynamic (>=1.0.7) to provide useful methods to read and write entities. These methods can be called with functionalities like pagination, filtering and sorting.

## Instalation
Get it on nuget: https://www.nuget.org/packages/DynamicLinqRepository/

	PM> Install-Package DynamicLinqRepository

## How to use
Create a new interface in order to configure your database repository. This interface must inherit from IEntityRepository<TEntity, TContext> interface, this interface depends on your Entity Framework DbContext. You can add any new methods here.
```C#
public interface IEntityRepository<TEntity> : IEntityRepository<TEntity, YourDbContext>
        where TEntity : class, IIdentifiableEntity, new()
    { }
```
Create a new class that will be used as your database repository. This class must inherit from the EntityRepository<TEntity, TContext> class and also from the IEntityRepository<TEntity> interface that you have just created.
```C#
public class EntityRepository<TEntity> : EntityRepository<TEntity, YourDbContext>, IEntityRepository<TEntity>
        where TEntity : class, IIdentifiableEntity, new()
    {
	public EntityRepository(bool lazyLoadingEnabled, bool proxyCreationEnabled)
            : base(lazyLoadingEnabled, proxyCreationEnabled)
        { }
    }
```
	
Create a new interface that will be used as your query results counter. This will later be used in the pagination service.
```C#
public interface IEntityCounter<TEntity> : IEntityCounter<TEntity, YourDbContext>
  where TEntity : class, IIdentifiableEntity, new()
{ }
```
	
Create a new class that must inherit from EntityCounter<TEntity, YourDbContext> and also from the interface that you have created.
```C#
public class EntityCounter<TEntity> : EntityCounter<TEntity, RepositoryContext>, IEntityCounter<TEntity>
  where TEntity : class, IIdentifiableEntity, new()
{
  public EntityCounter(bool lazyLoadingEnabled, bool proxyCreationEnabled)
    : base(lazyLoadingEnabled, proxyCreationEnabled)
  { }
}
```
	
All entity class must inherit from IIdentifiableEntity interface.
```C#
public class User : IIdentifiableEntity
{
  public int Id { get; set; }
  public string Name { get; set; }
  public int Age { get; set; }
  public int IdAddress { get; set; }
  public int IdDepartment { get; set; }

  public virtual Address Address { get; set; }
  public virtual Department Department { get; set; }
}	
```

Create a new repository and counter for **User** class. It will give you all the options to read and write to the User's table that you have configured on your DbContext.
```C#
var repository = new EntityRepository<User>(false, false)
var counter = new EntityCounter<User>(false, false);
```

You can enable lazy loading and proxy creation passing the boolean options in the repository constructor.

### Read Methods

Fetch all the data that matches the parameters of filtering, pagination and sorting. 

You can filter the data by passing a string list with the query Dynamic Linq clauses.
```C#
var clauses = new List<string>{ "Name.Contains(\"John\")", "Age > 20" };
var users = repository.FindAll(clauses);
```

The QueryLimits class defines the limits that are used in pagination and sorting. Use the counter to get the query result's total number of entities and use the QueryLimits class to fetch the current page's data.
```C#
var queryLimits = new QueryLimits
{
  Limit = 5, //number of lines in one page
  Page = 2, //number of current page
  OrderBy = "Name", //name of the property that the results will be sorted
  Orientation = "ASC" //the sort orientation (can be ASC or DESC)
};

var wrappedUsersCount = counter.Count(clauses);
if (wrappedUsersCount.Any())
{
  var usersCount = wrappedUsersCount.Single();
}
var users = repository.FindAll(queryLimits, clauses);
```

The returned count value is encapsulated in a **Maybe** class to avoid null reference errors.

You can fetch entity's nested entity properties.
```C#
var users = repository.FindAllIncluding(queryLimits, clauses, u => u.Address, u => u.Department);
var usersWithManager = repository.FindAllIncludingNestedProps(queryLimits, clauses, "Department.Manager");
```

### Write Methods

All the returned entities are encapsulated in a **Maybe** class to avoid null reference errors.

You can add a new entity with or without nested entity properties.
```C#
var user = new User();
var wrappedNewUser = repository.Add(user); //without adding nested entity properties
var wrappedNewUser = repository.AddWithNestedProperties(user); //adding nested entity properties
if(wrappedNewUser.Any()) 
{
  return wrappedNewUser.Single();
}
```

You can remove an entity by passing the id or object.
```C#
var wrappedRemovedUser = repository.Remove(1);
var wrappedRemovedUser = repository.Remove(user);
```

You can remove a range of entities by passing the objects or by passing the clauses.
```C#
var users = repository.FindAll(clauses);
repository.RemoveRange(users);
repository.RemoveRange(clauses);
```

You can update an entity or a range of entities.
```C#
var wrappedUpdatedUser = repository.Update(newUser);
repository.UpdateRange(newUsers);
```

## Dependencies

This project was implemented on top of these dependencies:

https://github.com/aspnet/EntityFramework6/wiki

https://github.com/kahanu/System.Linq.Dynamic

Many thanks!


# DynamicLinqRepository

Repository project that use Entity Framework and Dynamic Linq to provide useful methods for read and write entities with filtering, pagination and sorting.

## Instalation
Get it on nuget: https://www.nuget.org/packages/DynamicLinqRepository/

	PM> Install-Package DynamicLinqRepository

## How to use
Create a new interface that will configure your database repository. This interface most inherit from IEntityRepository<TEntity, TContext> interface, that depends of your DbContext. You can add any new methods here.
```C#
public interface IEntityRepository<TEntity> : IEntityRepository<TEntity, YourDbContext>
        where TEntity : class, IIdentifiableEntity, new()
    { }
```
Create a new class that will be used as your database repository. This class most inherit from the EntityRepository<TEntity, TContext> class and the IEntityRepository<TEntity> interface that you have just created.
```C#
public class EntityRepository<TEntity> : EntityRepository<TEntity, YourDbContext>, IEntityRepository<TEntity>
        where TEntity : class, IIdentifiableEntity, new()
    { }
```

Create a new repository for **User** class. It will give you all the options to read and write to the users table that you have configured on your DbContext.
```C#
var repository = new EntityRepository<User>(false, false)
```

You can enable lazy loading and proxy creation passing the boolean options in the repository constructor.

### Read Methods

Fetch all the data that matches the parameters of filtering, pagination and sorting. 

You can filter the data passing a string list that contains the dynamic linq clauses for that query.
```C#
var clauses = new List<string>{ "Name.Contains(\"John\"", "Age > 20" };
var users = repository.FindAll(clauses);
```

You can paginate and sort your query results by the QueryLimits class.
```C#
var queryLimits = new QueryLimits{
  Limit = 5, //number of lines in one page
  Page = 2, //number of current page
  OrderBy = "Name", //name of the property that the results will be sorted
  Orientation = "ASC" //the sort orientation (can be ASC or DESC)
}

var users = repository.FindAll(queryLimits, clauses);
```

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
var newUser = repository.Add(user); //without adding nested entity properties
var newUser = repository.AddWithNestedProperties(user); //with adding nested entity properties
if(newUser.Any()) {
  return newUser.Single();
}
```

You can remove an entity passing the id or object.
```C#
var removedUser = repository.Remove(1);
var removedUser = repository.Remove(user);
```

You can remove a range of entities passing the objects or passing the clauses.
```C#
var users = repository.FindAll(clauses);
repository.RemoveRange(users);
repository.RemoveRange(clauses);
```

You can update an entity or a range of entities.
```C#
var updatedUser = repository.Update(user);
repository.UpdateRange(users);
```


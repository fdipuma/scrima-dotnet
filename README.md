Scrima.NET
=====================

Scrima is a .NET library which parses an OData query uri into an object model which can be used to query custom data sources (`IQueryable` or not).

### Disclaimer and credits
This project is modified a combination of two existing Open source projects:
- [OpenApiQuery](https://github.com/Danielku15/OpenApiQuery) by [Daniel Kuschny](https://github.com/Danielku15) - MIT
  This early proof of concept project has greatly shown how to convert odata queries into `IQueryable` compatible expression
- [OpenData.Core](https://github.com/DwaynesWorld/OpenData) by [Kyle Dwayne Thompson](https://github.com/DwaynesWorld)
  An extremely useful way to parse an odata query into strongly typed model objects, that could be used to query any datasource

## How to install

- To use in a ASP.NET Core App (3.0+) `Install-Package Scrima.OData.AspNetCore`
- To use Swagger schemas with Swashbuckle `Install-Package Scrima.OData.Swashbuckle`
- To execute queries with EF Core `Install-Package Scrima.EntityFrameworkCore`

## Configuration

To configure the model binder to automatically parse and bind odata query string:

```csharp
public class Startup
{
    // ...
    
    public void ConfigureServices(IServiceCollection services)
    {
        // Add MVC services
        services.AddMvc();
        
        // Wire-up OData binder
        services.AddScrimaODataQuery();
        
        // Apply OData schema in swashbuckle (optional)
        
        services.AddSwaggerGen(s =>
        {
            s.SwaggerDoc("MyApi", new OpenApiInfo {Title = "MyApi", Version = "v1"});
            s.AddScrimaOData(p =>
            {
                // optionally configure which parameters to show
                // by item type
                p.ConfigureOptionsPerType = (options, type) =>
                {
                    options.AllowSkipToken = type == typeof(BlogPost);
                };
            });
        });
    }
    
    // ...
}
```

## How to use

In your controller(s), define a Get method which accepts a parameter of `ScrimaQueryOptions<T>`:

```csharp
public ActionResult GetAll(ScrimaQueryOptions<MyModel> query)
{
    // Implement query logic.
    
    var result = _context.Set<BlobPosts>().ToQueryResult(query);
    
    return Ok(new
    {
        items = result.Results,
        count = result.Count
    };
}
```

## Framework features

A partial list of supported features is shown above

| Feature             | Description                                                                   | Status            |
| ----------------    | ----------------------------------------------------------------------------- | ----------------- |
| `$filter`           | Filter result entities                                                        | Supported         |
| `$search`           | Filter result entities similarly to a full-text search                        | Partial support   |
| `$skip`             | Skip N elements in the result set                                             | Supported         |
| `$skiptoken`        | Skip elements in the result set by an opaque token (e.g. nosql)               | Supported         |
| `$top`              | Select the top N elements in the result set                                   | Supported         |
| `$count`            | Provide the total count of items in the data source (with filters applied)    | Supported         |
| `$orderby`          | Sorts elements using multiple properties                                      | Supported         |

Currently only filtering, ordering and paging are supported, other features like `$expand`, `$format` or aggregations are out of scope.

## OData URL Conventions Compatibility

| Feature                                               | Status |
| ---------------------------------------------         | -------|
| **5. Query Options**                                  | -                                                                                        |
| **5.1. System Query Options**                         | Partially Supported                                                                      |
| **5.1.1 Common Expression Syntax**                    | -                                                                                        |
| 5.1.1.1.1 Equals (`eq`)                               | Supported                                                                                |
| 5.1.1.1.2 Not Equals (`ne`)                           | Supported                                                                                |
| 5.1.1.1.3 Greater Than (`gt`)                         | Supported                                                                                |
| 5.1.1.1.4 Greater Than or Equal (`ge`)                | Supported                                                                                |
| 5.1.1.1.5 Less Than (`lt`)                            | Supported                                                                                |
| 5.1.1.1.6 Less Than or Equal (`le`)                   | Supported                                                                                |
| 5.1.1.1.7 And (`and`)                                 | Supported                                                                                |
| 5.1.1.1.8 Or (`or`)                                   | Supported                                                                                |
| 5.1.1.1.9 Not (`not`)                                 | Supported                                                                                |
| 5.1.1.1.10 Has (`has`)                                | Supported                                                                                |
| 5.1.1.2.1 Addition (`add`)                            | Supported                                                                                |
| 5.1.1.2.2 Subtraction (`sub`)                         | Supported                                                                                |
| 5.1.1.2.4 Multiplication (`mul`)                      | Supported                                                                                |
| 5.1.1.2.5 Division (`div`)                            | Supported                                                                                |
| 5.1.1.2.6 Modulo (`mod`)                              | Supported                                                                                |
| 5.1.1.5.1 concat (`concat()`)                         | Supported                                                                                |
| 5.1.1.5.2 contains (`contains()`)                     | Partially Supported (no collection contains collection)                                  |
| 5.1.1.5.3 endswith (`endswith()`)                     | Partially Supported (string)                                                             |
| 5.1.1.5.4 indexof (`indexof()`)                       | Partially Supported (string)                                                             |
| 5.1.1.5.5 length (`length()`)                         | Supported                                                                                |
| 5.1.1.5.6 startswith (`startswith()`)                 | Partially Supported (string)                                                             |
| 5.1.1.5.7 substring (`substring()`)                   | Supported                                                                                |
| 5.1.1.7 String Functions                              | Partially Supported (tolower, toupper, trim)                                             |
| 5.1.1.8 Date and Time Functions                       | Supported                                                                                |
| 5.1.1.9 Arithmetic Functions                          | Supported                                                                                |
| 5.1.1.10 Type Functions                               | Partially Supported (simple casts)                                                       |
| 5.1.1.14.1 Primitive Literals                         | Partially Supported (null, bool, int, double, single, string, dateTimeOffset, guid, long)|
| 5.1.1.14.2 Complex and Collection Literals            | Partially Supported (no aliases)                                                         |
| 5.1.1.14.3 null                                       | Supported                                                                                |
| 5.1.1.15 Path Expressions                             | Partially Supported                                                                      |
| 5.1.2 System Query Option $filter                     | Supported                                                                                |
| 5.1.3 System Query Option $expand                     | Supported                                                                                |
| 5.1.4 System Query Option $select                     | Supported                                                                                |
| 5.1.5 System Query Option $orderby                    | Supported                                                                                |
| 5.1.6 System Query Options $top and $skip             | Supported                                                                                |
| 5.1.7 System Query Option $count                      | Supported                                                                                |
| 5.1.8 System Query Option $search                     | Supported                                                                                |

## Naming
The name *Scrima* derives from the ancient [Italian sword art](https://en.wikipedia.org/wiki/Italian_martial_arts#Sistema_Scrima)
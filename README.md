# WEB API - MongoDB

## DESCRIÇÃO

Utilizando [ASP.NET](http://ASP.NET) Core, MongoDB, Swagger.

Uma API é uma interface de programação utilizada na montagem de aplicações. Ela faz o papel de recuperar somente os dados requisitados de um banco de dados.

Baixar o NuGet do MongoDb.Driver

## CRIAÇÃO DO PROJETO

![Untitled](https://github.com/Guilherme-Maciel/readme_images/blob/master/webApiMongo/Untitled.png)

# ESTRUTURA DO PROJETO

![Untitled](https://github.com/Guilherme-Maciel/readme_images/blob/master/webApiMongo/Untitled1.png)

# DEFINIÇÃO DOS DIRETÓRIOS

- CONTROLLER: Realiza o controle das COLLECTIONS do banco, contendo toda a interface da API, utilizando dos repositórios como dependência.
- DATA: Conexão e abstrações referentes ao banco.
- ENTITIES: Modelo de dados para as entidades do banco.
- REPOSITORIES: Contém todos os métodos da aplicação.

## RAIZ

`Startup.cs`

```csharp
using APImongodb.Data;
using APImongodb.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace APImongodb
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //Serviços implementados para utilização das interfaces
            services.AddScoped<ICatalogContext, CatalogContext>();
            services.AddScoped<IProductRepository, ProductRepository>();

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "APImongodb", Version = "v1" });
            });
        }
    }
}
```

## DATA

`ICatalogContext.cs`

```csharp
using APImongodb.Entities;
using MongoDB.Driver;

namespace APImongodb.Data
{
    public interface ICatalogContext
    {
        //Coleção dos produtos
        IMongoCollection<Product> Products { get; }
    }
}
```

`CatalogContextSeed.cs`

```csharp
using APImongodb.Entities;
using MongoDB.Driver;
using System.Collections.Generic;

namespace APImongodb.Data
{
    public class CatalogContextSeed
    {
        //Método que alimenta a COLLECTION em caso de não haver dados.
        public static void SeedData(IMongoCollection<Product> productCollection)
        {
            bool existProduct = productCollection.Find(p => true).Any();
            if (existProduct)
            {
                productCollection.InsertManyAsync(GetMyProducts());
            }
        }
        //Método contendo alguns registros teste para alimentar a COLLECTION
        private static IEnumerable<Product> GetMyProducts()
        {
            return new List<Product>()
            {
                new Product()
                {
                    Id = "602d2149e773f2a3990b47f5",
                    Name = "Iphone X",
                    Description = "Celular de alto padrão",
                    Category = "Smartphone",
                    Price = "R$ 3000,00",
                    Image = ""
                },
                new Product()
                {
                    Id = "602d2149e773f2a3990b47f5",
                    Name = "Samsung S10",
                    Description = "Celular de alto padrão",
                    Category = "Smartphone",
                    Price = "R$ 3000,00",
                    Image = ""
                },
            };
        }
    }
}
```

`CatalogContext.cs`

```csharp
using APImongodb.Entities;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

namespace APImongodb.Data
{
    public class CatalogContext : ICatalogContext
    {
        public CatalogContext(IConfiguration configuration)
        {
            //String de conexão
            var client = new MongoClient(configuration.GetValue<string>("DatabaseSettings:ConnectionString"));
            //database
            var database = client.GetDatabase(configuration.GetValue<string>("DatabaseSettings:DatabaseName"));
            //Collection
            Products = database.GetCollection<Product>(configuration.GetValue<string>("DatabaseSettings:CollectionName"));
    
            CatalogContextSeed.SeedData(Products);

        }
        public IMongoCollection<Product> Products { get;  }
    }
}
```

## ENTITIES

`Product.cs`

```csharp
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace APImongodb.Entities
{
    //Ignora se houver elementos extras na COLLECTION
    [BsonIgnoreExtraElements]
    public class Product
    {
        //Definição de uma entidade no MongoDB

        //Definição do ID padrão
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        //Nome dos atributos
        [BsonElement("Name")]
        public string Name { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public string Image { get; set; }
        public string Price { get; set; }

    }
}
```

## REPOSITORIES

`IProductRepository.cs`

```csharp
using APImongodb.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace APImongodb.Repositories
{
    public interface IProductRepository
    {
        //Assinaturas de Tasks para implementação na ProductRepository
        Task<IEnumerable<Product>> GetProducts();
        Task<Product> GetProduct(string id);
        Task<IEnumerable<Product>> GetProductsByName(string name);
        Task<IEnumerable<Product>> GetProductsByCategory(string categoryName);

        Task CreateProduct(Product product);
        Task<bool> UpdateProduct(Product product);
        Task<bool> DeleteProduct(string id);

    }
}
```

`ProductRepository.cs`

```csharp
using APImongodb.Data;
using APImongodb.Entities;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace APImongodb.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly ICatalogContext _context;

        public ProductRepository(ICatalogContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));

        }
        //Utilização das Tasks implementadas pela interface

        //SELECT *
        public async Task<IEnumerable<Product>> GetProducts()
        {
            return await _context.Products.Find(P => true).ToListAsync();
        }

        //INSERT INTO
        public async Task CreateProduct(Product product)
        {
             await _context.Products.InsertOneAsync(product);
        }

        //DELETE FROM
        public async Task<bool> DeleteProduct(string id)
        {
            FilterDefinition<Product> filter = Builders<Product>.Filter.Eq(p => p.Id, id);
            
            DeleteResult deleteResult = await _context.Products.DeleteOneAsync(filter);

            return deleteResult.IsAcknowledged && deleteResult.DeletedCount > 0;
        }

        //SELECT * WHERE
        public async Task<Product> GetProduct(string id)
        {
            return await _context.Products.Find(P => P.Id == id).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Product>> GetProductsByCategory(string categoryName)
        {
            FilterDefinition<Product> filter = Builders<Product>.Filter.Eq(p => p.Category, categoryName);

            return await _context.Products.Find(filter).ToListAsync();

        }

        public async Task<IEnumerable<Product>> GetProductsByName(string name)
        {
            FilterDefinition<Product> filter = Builders<Product>.Filter.Eq(p => p.Name, name);

            return await _context.Products.Find(filter).ToListAsync();
        }

        //UPGRADE SET
        public async Task<bool> UpdateProduct(Product product)
        {
            var updateResult = await _context.Products.ReplaceOneAsync(
                filter: g => g.Id == product.Id, replacement: product);

            return updateResult.IsAcknowledged && updateResult.ModifiedCount > 0;
        }
    }
}
```

## CONTROLLER

`CatalogController.cs`

```csharp
using APImongodb.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using APImongodb.Entities;

namespace APImongodb.Controllers
{
    [Route("api/v1/[controller]")]
    //habilita recursos à API
    [ApiController]
    public class CatalogController : ControllerBase
    {
        //Objeto do IProductRepository
        public readonly IProductRepository _repository;

        public CatalogController(IProductRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        //Interface e métodos da API
        
        //GET = SELECT
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Product>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts(int id)
        {
            var products = await _repository.GetProducts();
            return Ok(products);
        }

        [HttpGet("{id:length(24)}", Name = "GetProduct")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(Product), StatusCodes.Status200OK)]
        public async Task<ActionResult<Product>> GetProductById(string id)
        {
            var product = await _repository.GetProduct(id);
            if (product is null)
            {
                return NotFound();
            }
            return Ok(product);

        }

        [Route("[action]/{category}", Name = "GetProductByCategory")]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<Product>))]
        public async Task<ActionResult<IEnumerable<Product>>> GetProductByCategory(string category)
        {
            if (category is null)
            {
                return BadRequest("Invalid Category");
            }
            var products = await _repository.GetProductsByCategory(category);
            return Ok(products);
        }
        
        //POST = INSERT
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Product), StatusCodes.Status200OK)]
        public async Task<ActionResult<Product>> CreateProduct([FromBody] Product product)
        {
            if (product is null)
            {
                return BadRequest("Invalid Product");
            }
            await _repository.CreateProduct(product);
            return CreatedAtRoute("GetProduct", new { id = product.Id }, product);
        }

        //PUT = UPDATE
        [HttpPut]
        [ProducesResponseType(typeof(Product), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateProduct([FromBody] Product product)
        {
            if (product is null)
                return BadRequest("Invalid Product");

            return Ok(await _repository.UpdateProduct(product));
        }
 
        //DELETE
        [HttpDelete("{id:length(24)}", Name = "DeleteProduct")]
        [ProducesResponseType(typeof(Product), StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteProductById(string id)
        {
            return Ok(await _repository.DeleteProduct(id));
        }
    }

}
```

# REFERÊNCIAS

[.NET - Criando Microsserviços : API Catalogo com MongoDB - I](https://www.youtube.com/watch?v=ubCvfws1m4A)

[.NET - Criando Microsserviços : API Catalogo com MongoDB - II](https://www.youtube.com/watch?v=soTFJb-FSwo)

[.NET - Criando Microsserviços : API Catalogo com MongoDB - III](https://www.youtube.com/watch?v=gIHhCGyzQ_Q)

[.NET - Criando Microsserviços : API Catalogo com MongoDB - IV](https://www.youtube.com/watch?v=sGwC1i9YjZg)

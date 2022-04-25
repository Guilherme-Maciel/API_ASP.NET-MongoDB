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

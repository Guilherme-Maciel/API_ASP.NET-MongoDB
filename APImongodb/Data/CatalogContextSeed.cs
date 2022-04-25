using APImongodb.Entities;
using MongoDB.Driver;
using System.Collections.Generic;

namespace APImongodb.Data
{
    public class CatalogContextSeed
    {
        public static void SeedData(IMongoCollection<Product> productCollection)
        {
            bool existProduct = productCollection.Find(p => true).Any();
            if (existProduct)
            {
                productCollection.InsertManyAsync(GetMyProducts());
            }
        }
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

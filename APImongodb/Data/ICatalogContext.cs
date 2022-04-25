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

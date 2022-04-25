using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace APImongodb.Entities
{
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

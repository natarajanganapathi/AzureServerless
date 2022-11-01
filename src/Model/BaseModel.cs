namespace Incubation.AzConf.Model;

[BsonIgnoreExtraElements]
public class BaseModel
{
    [BsonElement("_id")]
    [BsonId]
    [BsonIgnoreIfDefault]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }
}
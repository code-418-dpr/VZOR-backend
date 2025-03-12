namespace VZOR.Images.Infrastructure.Options;

public class MongoDatabaseSettings
{
    public static string Mongo = nameof(Mongo);
    
    public string ConnectionString { get; set; } = null!;
    public string DatabaseName { get; set; } = null!;

    public string ImagesCollectionName { get; set; } = null!;
}
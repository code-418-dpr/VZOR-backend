namespace VZOR.Images.Infrastructure.Options;

public class ElasticsearchOptions
{
    public const string ELASTICSEARCH = "Elasticsearch";

    public string Uri { get; set; }
    public string DefaultIndex { get; set; }
}
using MongoDB.Bson.Serialization.Attributes;
using VZOR.SharedKernel;
using VZOR.SharedKernel.Errors;

namespace VZOR.Images.Domain;

public class Image
{
    [BsonId]
    public required string Id { get; init; }
    [BsonRequired]
    public required string UserId { get; init; }
    [BsonRequired]
    public required DateTime UploadDate { get; init; }
    [BsonRequired]
    public required string UploadLink { get; init; }
    [BsonElement]
    public string? ProcessingResult { get; set; }
}
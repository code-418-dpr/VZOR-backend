using VZOR.SharedKernel;
using VZOR.SharedKernel.Errors;

namespace VZOR.Images.Domain;

public class Visitor
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? MiddleName { get; set; }

    public List<string> VisitorPhotos { get; set; } = [];

    public Result AddPhotos(IEnumerable<string> photos)
    {
        foreach (var photo in photos)
        {
            if (VisitorPhotos.Contains(photo))
                return Error.Conflict("add.photo.conflict", "This photo is already added");
            
            VisitorPhotos.Add(photo);
        }
        
        return Result.Success();
    }
}
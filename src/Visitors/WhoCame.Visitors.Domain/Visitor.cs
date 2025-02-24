namespace WhoCame.Visitors.Domain;

public class Visitor
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? MiddleName { get; set; }

    public List<string> VisitorPhotos { get; set; } = [];
}
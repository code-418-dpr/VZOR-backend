namespace WhoCame.Visitors.Domain;

public class Visitor
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string MiddleName { get; set; }

    public List<string> VisitorPhotos { get; set; } = [];
}
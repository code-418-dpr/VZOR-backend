namespace VZOR.Images.Domain;

public class ProcessingResult
{
    public string Description { get; set;  } = string.Empty;
    public List<string> Objects { get; set; } = [];
    public string Text { get; set; } = String.Empty;
}
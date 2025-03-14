namespace VZOR.Core.Dtos;

public class ProcessingResultDto
{
    public string Description { get; set;  } = string.Empty;
    public List<string> Objects { get; set; } = [];
    public string Text { get; set; } = String.Empty;
}
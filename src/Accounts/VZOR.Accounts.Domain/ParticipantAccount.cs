namespace VZOR.Accounts.Domain;

public class ParticipantAccount
{
    public static readonly string Participant = nameof(Participant);
    
    private ParticipantAccount(){}

    public ParticipantAccount(User user, string firstName, string lastName, string? middleName)
    {
        Id = Guid.NewGuid();
        User = user;
        FirstName = firstName;
        LastName = lastName;
        MiddleName = middleName;
    }
    
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? MiddleName { get; set; }
    
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public User User { get; set; }
}
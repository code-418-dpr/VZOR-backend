namespace WhoCame.Accounts.Domain;

public class ParticipantAccount
{
    public static readonly string Participant = nameof(Participant);
    
    private ParticipantAccount(){}

    public ParticipantAccount(User user)
    {
        Id = Guid.NewGuid();
        User = user;
    }
    
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public User User { get; set; }
}
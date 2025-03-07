﻿namespace VZOR.Accounts.Domain;

public class ParticipantAccount
{
    public static readonly string Participant = nameof(Participant);
    
    private ParticipantAccount(){}

    public ParticipantAccount(User user, string name)
    {
        Id = Guid.NewGuid();
        User = user;
        Name = name;
    }
    
    public string Name { get; set; } = string.Empty;
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public User User { get; set; }
}
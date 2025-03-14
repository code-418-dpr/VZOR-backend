﻿using Microsoft.AspNetCore.Identity;

namespace VZOR.Accounts.Domain;

public class User:IdentityUser<Guid>
{
    private User()
    {
        
    }
    
    private List<Role> _roles = [];
    public IReadOnlyList<Role> Roles => _roles;
    public Guid? ParticipantAccountId { get; set; }
    public ParticipantAccount? ParticipantAccount { get; set; }
    public AdminProfile? AdminProfile { get; set; }
    public bool IsActive { get; set; }
    
    public static User CreateAdmin(string userName, string email, Role role)
    {
        return new User
        {
            UserName = userName,
            Email = email,
            _roles = [role],
            IsActive = true
        };
    }
    
    public static User CreateParticipant(
        string email,
        Role role)
    {
        return new User
        {
            Email = email,
            _roles = [role],
            IsActive = true
        };
    }
}


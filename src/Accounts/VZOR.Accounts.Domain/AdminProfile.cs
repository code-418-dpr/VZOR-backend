namespace VZOR.Accounts.Domain;

public class AdminProfile
{
    public const string ADMIN = "Admin";
 
    private AdminProfile(){}
    
    public AdminProfile(User user)
    {
        Id = Guid.NewGuid();
        User = user;
    }
    
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public User? User { get; set; }
}
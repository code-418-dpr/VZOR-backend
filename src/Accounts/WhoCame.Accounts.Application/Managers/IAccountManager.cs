using WhoCame.Accounts.Domain;
using WhoCame.SharedKernel;

namespace WhoCame.Accounts.Application.Managers;

public interface IAccountManager
{
    Task CreateAdminAccount(AdminProfile adminProfile);
    Task<Result> CreateParticipantAccount(
        ParticipantAccount participantAccount, CancellationToken cancellationToken = default);
}
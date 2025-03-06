using VZOR.Accounts.Domain;
using VZOR.SharedKernel;

namespace VZOR.Accounts.Application.Managers;

public interface IAccountManager
{
    Task CreateAdminAccount(AdminProfile adminProfile);
    Task<Result> CreateParticipantAccount(
        ParticipantAccount participantAccount, CancellationToken cancellationToken = default);
}
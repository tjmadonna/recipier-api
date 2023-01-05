using Api.Features.V1.Core;
using Api.Features.V1.User.Models;

namespace Api.Features.V1.User;

public interface IUserService
{
    public Task<Result<UserInfo>> CreateUserAsync(Models.User user);

    public Task<Result<UserInfo>> GetUserInfoAsync(Guid userId);
}
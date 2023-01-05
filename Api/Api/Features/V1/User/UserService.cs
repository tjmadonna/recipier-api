using Api.Data.Entities;
using Api.Features.V1.Core;
using Api.Features.V1.User.Models;
using Microsoft.AspNetCore.Identity;

namespace Api.Features.V1.User;

public class UserService : IUserService
{
    private readonly UserManager<AppUser> _userManager;

    private readonly ILogger<UserController> _logger;

    public UserService(UserManager<AppUser> userManager, ILogger<UserController> logger)
    {
        _userManager = userManager;
        _logger = logger;
    }

    public async Task<Result<UserInfo>> CreateUserAsync(Models.User user)
    {
        var existingUser = await _userManager.FindByEmailAsync(user.Email);
        if (existingUser != null)
            return Result.Failure<UserInfo>("User with provided email already exists.");

        var newUser = new AppUser
        {
            Id = Guid.NewGuid(),
            Email = user.Email,
            UserName = user.Email,
        };
        var createdResult = await _userManager.CreateAsync(newUser, user.Password);
        if (!createdResult.Succeeded)
        {
            var error = createdResult.Errors.FirstOrDefault()?.Description ?? string.Empty;
            return Result.Failure<UserInfo>(error);
        }

        return Result.Success(new UserInfo
        {
            Id = newUser.Id,
            Email = newUser.Email
        });
    }

    public async Task<Result<UserInfo>> GetUserInfoAsync(Guid userId)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null || user.Email == null)
            return Result.Failure<UserInfo>("User with provided ID does not exist.");

        return Result.Success(new UserInfo
        {
            Id = user.Id,
            Email = user.Email
        });
    }
}
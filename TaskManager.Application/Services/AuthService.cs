using FluentValidation;
using Mapster;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TaskManager.Application.DTOs;
using TaskManager.Application.DTOs.Auth.Request;
using TaskManager.Application.DTOs.Auth.Response;
using TaskManager.Application.Interfaces;
using TaskManager.Application.Validators;
using TaskManager.Application.Validators.Auth;
using TaskManager.Core.Constants;
using TaskManager.Core.Entities;
using TaskManager.Core.Interfaces;

namespace TaskManager.Application.Services;
public class AuthService : IAuthService
{   
    private readonly IUserRepository _userRepository;
    private readonly IConfiguration _configuration;
    public AuthService(IUserRepository userRepository, IConfiguration configuration)
    {
        _userRepository = userRepository;
        _configuration = configuration;
    }
    public async Task<ResultResponse<string>> CreateUserAsync(CreateUserRequest request)
    {
        request.Name = request.Name.Trim();
        request.Email = request.Email.Trim().ToLowerInvariant();

        CreateUserValidator validator = new CreateUserValidator();
        await validator.ValidateAndThrowAsync(request);

        bool userExists = await _userRepository.UserExistsAsync(request.Email);

        if (userExists)
            return ResultResponse<string>.Failure(string.Format(Messages.RESOURCE_ALREADY_EXISTS, "Usuário"));

        var newUser = request.Adapt<User>();

        newUser.HashPassword = BCrypt.Net.BCrypt.HashPassword(request.Password);

        await _userRepository.AddUserAsync(newUser);
        return ResultResponse<string>.Success(string.Format(Messages.OPERATION_SUCCESS, "Usuário criado"));
    }

    public async Task<ResultResponse<LoginResponse>> LoginAsync(LoginRequest request)
    {
        request.Email = request.Email.Trim().ToLowerInvariant();

        LoginValidator validator = new LoginValidator();
        await validator.ValidateAndThrowAsync(request);

        User? user = await _userRepository.GetUserByEmailAsync(request.Email);

        if (user == null)
            return ResultResponse<LoginResponse>.Failure(Messages.INVALID_CREDENTIALS);

        bool isValidPassword = BCrypt.Net.BCrypt.Verify(request.Password, user.HashPassword);

        if (!isValidPassword)
            return ResultResponse<LoginResponse>.Failure(Messages.INVALID_CREDENTIALS);

        LoginResponse response = user.Adapt<LoginResponse>();
        response.Token = GenerateToken(user);

        return ResultResponse<LoginResponse>.Success(response, Messages.LOGIN_SUCCESSFULLY);
    }

    private string GenerateToken(User user)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email!),
            new Claim(ClaimTypes.Name, user.Name!),
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));

        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(Constants.JWT_EXPIRATION_HOURS),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}

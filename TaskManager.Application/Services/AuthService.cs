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
    public async Task<ResultResponse<CreateUserRequest>> CreateUserAsync(CreateUserRequest request)
    {
        CreateUserValidator validator = new CreateUserValidator();
        await validator.ValidateAndThrowAsync(request);

        bool userExists = await _userRepository.UserExistsAsync(request.Email);

        if (userExists)
            return ResultResponse<CreateUserRequest>.Failure(string.Format(Messages.RESOURCE_ALREADY_EXISTS, "Usuário"));

        var newUser = request.Adapt<User>();

        newUser.HashPassword = BCrypt.Net.BCrypt.HashPassword(request.Password);

        await _userRepository.AddUserAsync(newUser);
        return ResultResponse<CreateUserRequest>.Success(string.Format(Messages.OPERATION_SUCCESS, "Usuário criado"));
    }

    public async Task<ResultResponse<LoginResponse>> LoginAsync(LoginRequest userLoginDto)
    {
        User? user = await _userRepository.GetUserByEmailAsync(userLoginDto.Email);

        if (user == null)
            return ResultResponse<LoginResponse>.Failure(Messages.INVALID_CREDENTIALS);

        bool isValidPassword = BCrypt.Net.BCrypt.Verify(userLoginDto.Password, user.HashPassword);

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
            expires: DateTime.UtcNow.AddHours(2),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}

using Mapster;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TaskManager.Application.DTOs;
using TaskManager.Application.Interfaces;
using TaskManager.Application.Validators;
using TaskManager.Core.Constants;
using TaskManager.Core.Entities;
using TaskManager.Core.Interfaces;

namespace TaskManager.Application.Services;
public class AuthService : IAuthService
{   
    private readonly IAuthRepository _authRepository;
    private readonly IConfiguration _configuration;
    public AuthService(IAuthRepository authRepository, IConfiguration configuration)
    {
        _authRepository = authRepository;
        _configuration = configuration;
    }
    public async Task<ResultDto<CreateUserDto>> RegisterAsync(CreateUserDto userRegisterDto)
    {
        CreateUserValidator validator = new CreateUserValidator();

        var validationResult = await validator.ValidateAsync(userRegisterDto);

        if (!validationResult.IsValid)
            return ResultDto<CreateUserDto>.ValidationFailure(validationResult.Errors);

        bool exists = await _authRepository.ExistsAsync(userRegisterDto.Email);

        if (exists)
            return ResultDto<CreateUserDto>.Failure(string.Format(Messages.USER_ALREADY_EXISTS));

        var newUser = userRegisterDto.Adapt<User>();

        newUser.Id = Guid.NewGuid();
        newUser.CreatedAt = DateTime.UtcNow;
        newUser.HashPassword = BCrypt.Net.BCrypt.HashPassword(userRegisterDto.Password);

        await _authRepository.AddAsync(newUser);
        return ResultDto<CreateUserDto>.Success(string.Format(Messages.USER_CREATED_SUCCESSFULLY));
    }

    public async Task<ResultDto<LoginResponseDto>> LoginAsync(UserLoginDto userLoginDto)
    {
        User? user = await _authRepository.GetByEmailAsync(userLoginDto.Email);

        if (user == null)
            return ResultDto<LoginResponseDto>.Failure(Messages.USER_NOT_FOUND);

        bool validPassword = BCrypt.Net.BCrypt.Verify(userLoginDto.Password, user.HashPassword);

        if (!validPassword)
            return ResultDto<LoginResponseDto>.Failure(Messages.USER_OR_PASSWORD_INVALID);

        string userToken = GenerateToken(user);

        LoginResponseDto response = user.Adapt<LoginResponseDto>();

        response.Token = userToken;

        return ResultDto<LoginResponseDto>.Success(response, Messages.LOGIN_SUCCESSFULLY);
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

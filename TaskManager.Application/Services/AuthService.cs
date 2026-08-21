using FluentValidation;
using Mapster;
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
    private readonly IJwtService _jwtService;
    public AuthService(IAuthRepository authRepository, IJwtService jwtService)
    {
        _authRepository = authRepository;
        _jwtService = jwtService;
    }
    public async Task<ResultDto<CreateUserDto>> RegisterAsync(CreateUserDto userRegisterDto)
    {
        CreateUserValidator validator = new CreateUserValidator();

        var validationResult = await validator.ValidateAsync(userRegisterDto);

        if (!validationResult.IsValid)
            return ResultDto<CreateUserDto>.ValidationFailure(validationResult.Errors);

        bool exists = await _authRepository.ExistsAsync(userRegisterDto.Email);

        if (exists)
            return ResultDto<CreateUserDto>.Failure(string.Format(Messages.UserAlreadyExists));

        var newUser = userRegisterDto.Adapt<User>();

        newUser.Id = Guid.NewGuid();
        newUser.CreatedAt = DateTime.UtcNow;
        newUser.HashPassword = BCrypt.Net.BCrypt.HashPassword(userRegisterDto.Password);

        await _authRepository.AddAsync(newUser);
        return ResultDto<CreateUserDto>.Success(string.Format(Messages.UserCreatedSucessfully));
    }

    public async Task<ResultDto<LoginResponseDto>> LoginAsync(UserLoginDto userLoginDto)
    {
        User? user = await _authRepository.GetByEmailAsync(userLoginDto.Email);

        if (user == null)
            return ResultDto<LoginResponseDto>.Failure(Messages.UserNotFound);

        bool validPassword = BCrypt.Net.BCrypt.Verify(userLoginDto.Password, user.HashPassword);

        if (!validPassword)
            return ResultDto<LoginResponseDto>.Failure(Messages.UserOrPasswordInvalid);

        string userToken = _jwtService.GenerateToken(user);

        LoginResponseDto response = user.Adapt<LoginResponseDto>();

        response.Token = userToken;

        return ResultDto<LoginResponseDto>.Success(response);
    }
}

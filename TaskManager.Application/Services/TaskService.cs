using FluentValidation;
using Mapster;
using TaskManager.Application.DTOs;
using TaskManager.Application.Interfaces;
using TaskManager.Application.Validators;
using TaskManager.Core.Constants;
using TaskManager.Core.Entities;
using TaskManager.Core.Interfaces;
using TaskManager.Core.Shared;

namespace TaskManager.Application.Services;
public class TaskService : ITaskService
{
    private readonly ITaskRepository _taskRepository;

    public TaskService(ITaskRepository taskRepository)
    {
        _taskRepository = taskRepository;
    }
    public async Task<ResultDto<List<TaskResponseDto>>> GetTasksByTitle(string title, Guid userId)
    {
        List<TaskEntity> tasks = await _taskRepository.GetTaskByTitle(title, userId);

        if (tasks.Count == 0)
            return ResultDto<List<TaskResponseDto>>.Failure(string.Format(Messages.TASK_NOT_FOUND));

        List<TaskResponseDto> listOfTasks = tasks.Adapt<List<TaskResponseDto>>();

        return ResultDto<List<TaskResponseDto>>.Success(listOfTasks);
    }

    public async Task<ResultDto<TaskResponseDto>> GetTaskById(Guid taskId, Guid userId)
    {
        if (taskId == Guid.Empty || userId == Guid.Empty)
            return ResultDto<TaskResponseDto>.Failure(string.Format(Messages.TASK_FETCH_FAILED));

        TaskEntity task = await _taskRepository.GetTaskById(taskId, userId);

        if (task == null)
            return ResultDto<TaskResponseDto>.Failure(string.Format(Messages.TASK_NOT_FOUND));

        TaskResponseDto taskDto = task.Adapt<TaskResponseDto>();

        return ResultDto<TaskResponseDto>.Success(taskDto);
    }

    public async Task<ResultDto<PagedResultDto<TaskResponseDto>>> GetAllTasks(Guid userId, PagedParamsDto pagedParams)
    {
        if (userId == Guid.Empty || pagedParams == null)
            return ResultDto<PagedResultDto<TaskResponseDto>>.Failure(string.Format(Messages.TASK_NOT_FOUND));

        var (tasks, totalCount) = await _taskRepository.GetAllTasks(userId, pagedParams);

        List<TaskResponseDto> tasksDtos = tasks.Adapt<List<TaskResponseDto>>();

        if (tasksDtos.Count == 0)
            return ResultDto<PagedResultDto<TaskResponseDto>>.Failure(string.Format(Messages.TASK_NOT_FOUND));

        PagedResultDto<TaskResponseDto> pagedResult = new PagedResultDto<TaskResponseDto>(tasksDtos, pagedParams.PageNumber, pagedParams.PageSize, totalCount);

        return ResultDto<PagedResultDto<TaskResponseDto>>.Success(pagedResult);
    }

    public async Task<ResultDto<TaskEntity>> AddTaskAsync(CreateTaskDto task, Guid userId)
    {
        if (task == null || userId == Guid.Empty)
            return ResultDto<TaskEntity>.Failure(string.Format(Messages.TASK_CREATION_FAILED));

        CreateTaskValidator validator = new CreateTaskValidator();

        var validationResult = await validator.ValidateAsync(task);

        if (!validationResult.IsValid)
            return ResultDto<TaskEntity>.ValidationFailure(validationResult.Errors);

        TaskEntity newTask = task.Adapt<TaskEntity>();
        newTask.UserId = userId;

        await _taskRepository.AddTaskAsync(newTask);
        return ResultDto<TaskEntity>.Success(string.Format(Messages.TASK_CREATED_SUCCESSFULLY));
    }
    public async Task<ResultDto<TaskResponseDto>> EditTaskAsync(EditTaskDto dto, Guid userId)
    {
        if (dto.Id == Guid.Empty || userId == Guid.Empty)
            return ResultDto<TaskResponseDto>.Failure(string.Format(Messages.TASK_UPDATE_FAILED));

        EditTaskValidator validator = new EditTaskValidator();

        var validationResult = await validator.ValidateAsync(dto);

        if (!validationResult.IsValid)
            return ResultDto<TaskResponseDto>.ValidationFailure(validationResult.Errors);

        TaskEntity task = await _taskRepository.GetTaskById(dto.Id, userId);

        if (task == null)
            return ResultDto<TaskResponseDto>.Failure(string.Format(Messages.TASK_NOT_FOUND));

        dto.Adapt(task);

        TaskResponseDto taskDto = task.Adapt<TaskResponseDto>();

        await _taskRepository.EditTaskAsync(task);

        return ResultDto<TaskResponseDto>.Success(taskDto);
    }

    public async Task<ResultDto<string>> DeleteTaskAsync(Guid taskId, Guid userId)
    {
        if (taskId == Guid.Empty || userId == Guid.Empty)
            return ResultDto<string>.Failure(string.Format(Messages.TASK_DELETION_FAILED));

        TaskEntity task = await _taskRepository.GetTaskById(taskId, userId);

        if (task == null)
            return ResultDto<string>.Failure(string.Format(Messages.TASK_NOT_FOUND));

        await _taskRepository.DeleteTaskAsync(task);
        return ResultDto<string>.Success(string.Format(Messages.TASK_DELETED_SUCCESSFULLY));
    }
}


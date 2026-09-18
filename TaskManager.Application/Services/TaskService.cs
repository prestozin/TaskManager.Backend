using FluentValidation;
using Mapster;
using TaskManager.Application.DTOs;
using TaskManager.Application.DTOs.Task.Request;
using TaskManager.Application.DTOs.Task.Response;
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

    public async Task<ResultDto<TaskResponse>> GetTaskAsync(Guid taskId, Guid userId)
    {
        if (taskId == Guid.Empty || userId == Guid.Empty)
            return ResultDto<TaskResponse>.Failure(string.Format(Messages.TASK_FETCH_FAILED));

        TaskEntity? task = await _taskRepository.GetTaskByIdAsync(taskId, userId);

        if (task == null)
            return ResultDto<TaskResponse>.Failure(string.Format(Messages.TASK_NOT_FOUND));

        TaskResponse taskDto = task.Adapt<TaskResponse>();

        return ResultDto<TaskResponse>.Success(taskDto);
    }

    public async Task<ResultDto<PagedResultDto<TaskResponse>>> GetPagedAsync(Guid userId, TaskPagedParams pagedParams)
    {
        if (userId == Guid.Empty || pagedParams == null)
            return ResultDto<PagedResultDto<TaskResponse>>.Failure(string.Format(Messages.TASK_NOT_FOUND));

        var (tasks, totalCount) = await _taskRepository.GetPaged(userId, pagedParams);

        List<TaskResponse> tasksDtos = tasks.Adapt<List<TaskResponse>>();

        if (tasksDtos.Count == 0)
            return ResultDto<PagedResultDto<TaskResponse>>.Failure(string.Format(Messages.TASK_NOT_FOUND));

        PagedResultDto<TaskResponse> pagedResult = new PagedResultDto<TaskResponse>(tasksDtos, pagedParams.PageNumber, pagedParams.PageSize, totalCount);

        return ResultDto<PagedResultDto<TaskResponse>>.Success(pagedResult);
    }

    public async Task<ResultDto<string>> AddTaskAsync(CreateTaskRequest task, Guid userId)
    {
        if (task == null || userId == Guid.Empty)
            return ResultDto<string>.Failure(Messages.TASK_CREATION_FAILED);

        CreateTaskValidator validator = new CreateTaskValidator();
        await validator.ValidateAndThrowAsync(task);

        TaskEntity newTask = task.Adapt<TaskEntity>();
        newTask.UserId = userId;

        await _taskRepository.AddTaskAsync(newTask);

        TaskResponse response = newTask.Adapt<TaskResponse>();

        return ResultDto<string>.Success(Messages.TASK_CREATED_SUCCESSFULLY);
    }
    public async Task<ResultDto<string>> EditTaskAsync(EditTaskRequest dto, Guid userId)
    {
        if (dto.Id == Guid.Empty || userId == Guid.Empty)
            return ResultDto<string>.Failure(Messages.TASK_UPDATE_FAILED);

        EditTaskValidator validator = new EditTaskValidator();
        await validator.ValidateAndThrowAsync(dto);

        TaskEntity? task = await _taskRepository.GetTaskByIdAsync(dto.Id, userId);

        if (task == null)
            return ResultDto<string>.Failure(Messages.TASK_NOT_FOUND);

        dto.Adapt(task);

        TaskResponse taskDto = task.Adapt<TaskResponse>();

        await _taskRepository.EditTaskAsync(task);

        return ResultDto<string>.Success(Messages.TASK_UPDATED_SUCCESSFULLY);
    }

    public async Task<ResultDto<string>> DeleteTaskAsync(DeleteTaskRequest request, Guid userId)
    {
        if (request.TaskId == null ||request.TaskId.Count == 0 || request.TaskId.Any(id => id == Guid.Empty) || userId == Guid.Empty)
            return ResultDto<string>.Failure(Messages.TASK_DELETION_FAILED);
        

        List<TaskEntity> deletedTasks = [];

        foreach (Guid taskId in request.TaskId)
        {
            TaskEntity? task = await _taskRepository.GetTaskByIdAsync(taskId, userId);

            if (task == null) continue;

            await _taskRepository.DeleteTaskAsync(task);
            deletedTasks.Add(task);
        }

        if (deletedTasks.Count == 0)
            return ResultDto<string>.Failure(Messages.TASK_NOT_FOUND);

        return ResultDto<string>.Success(GetDeletionMessage(request.TaskId, deletedTasks));
    }

    private static string GetDeletionMessage(List<Guid> taskIds,List<TaskEntity> deletedTasks)
    {
        int deletedCount = deletedTasks.Count;
        int notFoundCount = taskIds.Count - deletedCount;

        if (notFoundCount > 0)
            return $"{deletedCount} tarefa(s) excluída(s). {notFoundCount} tarefa(s) não encontrada(s).";

        return $"{deletedCount} tarefa(s) excluída(s) com sucesso.";
    }
    public async Task<ResultDto<TaskSelectablesResponse>> GetSelectablesAsync()
    {
        List<Core.Entities.TaskStatus> statuses = await _taskRepository.GetTaskStatusesAsync();
        List<TaskPriority> priorities = await _taskRepository.GetTaskPrioritiesAsync();

        if (statuses == null || priorities == null)
            return ResultDto<TaskSelectablesResponse>.Failure(string.Format(Messages.FIELD_NOT_FOUND, "Selectables"));

        TaskSelectablesResponse selectablesDto = new TaskSelectablesResponse
        {
            Status = statuses,
            Priority = priorities
        };

        return ResultDto<TaskSelectablesResponse>.Success(selectablesDto);
    }
}


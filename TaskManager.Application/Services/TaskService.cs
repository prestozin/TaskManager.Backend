using FluentValidation;
using Mapster;
using TaskManager.Application.DTOs;
using TaskManager.Application.DTOs.Task.Report;
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
    private readonly ICurrentUserContext _currentUserContext;

    private Guid UserId => _currentUserContext.UserId;


    public TaskService(ITaskRepository taskRepository, ICurrentUserContext currentUserContext)
    {
        _taskRepository = taskRepository;
        _currentUserContext = currentUserContext;

    }

    public async Task<ResultResponse<TaskResponse>> GetTaskByIdAsync(Guid taskId)
    {
        if (taskId == Guid.Empty || UserId == Guid.Empty)
            return ResultResponse<TaskResponse>.Failure(string.Format(Messages.TASK_FETCH_FAILED));

        TaskEntity? task = await _taskRepository.GetTaskByIdAsync(taskId, UserId);

        if (task == null)
            return ResultResponse<TaskResponse>.Failure(string.Format(Messages.TASK_NOT_FOUND));

        TaskResponse taskDto = task.Adapt<TaskResponse>();

        return ResultResponse<TaskResponse>.Success(taskDto);
    }

    public async Task<ResultResponse<PagedResultDto<TaskResponse>>> GetPagedAsync(TaskPagedParams pagedParams)
    {
        if (UserId == Guid.Empty || pagedParams == null)
            return ResultResponse<PagedResultDto<TaskResponse>>.Failure(string.Format(Messages.TASK_NOT_FOUND));

        var (tasks, totalCount) = await _taskRepository.GetPagedAsync(UserId, pagedParams);

        List<TaskResponse> tasksDtos = tasks.Adapt<List<TaskResponse>>();

        if (tasksDtos.Count == 0)
            return ResultResponse<PagedResultDto<TaskResponse>>.Failure(string.Format(Messages.TASK_NOT_FOUND));

        PagedResultDto<TaskResponse> pagedResult = new PagedResultDto<TaskResponse>(tasksDtos, pagedParams.PageNumber, pagedParams.PageSize, totalCount);

        return ResultResponse<PagedResultDto<TaskResponse>>.Success(pagedResult);
    }

    public async Task<ResultResponse<string>> CreateTaskAsync(CreateTaskRequest task)
    {
        if (task == null || UserId == Guid.Empty)
            return ResultResponse<string>.Failure(Messages.TASK_CREATION_FAILED);

        CreateTaskValidator validator = new CreateTaskValidator();
        await validator.ValidateAndThrowAsync(task);

        TaskEntity newTask = task.Adapt<TaskEntity>();
        newTask.UserId = UserId;

        await _taskRepository.CreateTaskAsync(newTask);

        TaskResponse response = newTask.Adapt<TaskResponse>();

        return ResultResponse<string>.Success(Messages.TASK_CREATED_SUCCESSFULLY);
    }
    public async Task<ResultResponse<string>> EditTaskAsync(EditTaskRequest dto)
    {
        if (dto.Id == Guid.Empty || UserId == Guid.Empty)
            return ResultResponse<string>.Failure(Messages.TASK_UPDATE_FAILED);

        EditTaskValidator validator = new EditTaskValidator();
        await validator.ValidateAndThrowAsync(dto);

        TaskEntity? task = await _taskRepository.GetTaskByIdAsync(dto.Id, UserId);

        if (task == null)
            return ResultResponse<string>.Failure(Messages.TASK_NOT_FOUND);

        dto.Adapt(task);

        TaskResponse taskDto = task.Adapt<TaskResponse>();

        await _taskRepository.EditTaskAsync(task);

        return ResultResponse<string>.Success(Messages.TASK_UPDATED_SUCCESSFULLY);
    }

    public async Task<ResultResponse<string>> DeleteTaskAsync(DeleteTaskRequest request)
    {
        if (request.TaskId == null ||request.TaskId.Count == 0 || request.TaskId.Any(id => id == Guid.Empty) || UserId == Guid.Empty)
            return ResultResponse<string>.Failure(Messages.TASK_DELETION_FAILED);
        

        List<TaskEntity> deletedTasks = [];

        foreach (Guid taskId in request.TaskId)
        {
            TaskEntity? task = await _taskRepository.GetTaskByIdAsync(taskId, UserId);

            if (task == null) continue;

            await _taskRepository.DeleteTaskAsync(task);
            deletedTasks.Add(task);
        }

        if (deletedTasks.Count == 0)
            return ResultResponse<string>.Failure(Messages.TASK_NOT_FOUND);

        return ResultResponse<string>.Success(GetDeletionMessage(request.TaskId, deletedTasks));
    }

    private static string GetDeletionMessage(List<Guid> taskIds,List<TaskEntity> deletedTasks)
    {
        int deletedCount = deletedTasks.Count;
        int notFoundCount = taskIds.Count - deletedCount;

        if (notFoundCount > 0)
            return $"{deletedCount} tarefa(s) excluída(s). {notFoundCount} tarefa(s) não encontrada(s).";

        return $"{deletedCount} tarefa(s) excluída(s) com sucesso.";
    }

    public async Task<ResultResponse<TaskSelectablesResponse>> GetSelectablesAsync()
    {
        List<Core.Entities.TaskStatus> statuses = await _taskRepository.GetTaskStatusesAsync();
        List<TaskPriority> priorities = await _taskRepository.GetTaskPrioritiesAsync();

        if (statuses == null || priorities == null)
            return ResultResponse<TaskSelectablesResponse>.Failure(string.Format(Messages.FIELD_NOT_FOUND, "Selectables"));

        TaskSelectablesResponse selectablesDto = new TaskSelectablesResponse
        {
            Status = statuses,
            Priority = priorities
        };

        return ResultResponse<TaskSelectablesResponse>.Success(selectablesDto);
    }

    public async Task<ResultResponse<TaskReportResponse>> GetReportAsync(ReportPagedParams reportParams)
    {
        if (UserId == Guid.Empty || reportParams == null)
            return ResultResponse<TaskReportResponse>.Failure(Messages.TASK_NOT_FOUND);

        var tasks = (await _taskRepository.GetReportAsync(UserId, reportParams)).ToList();

        var pagedParams = reportParams.Adapt<TaskPagedParams>();

        var (pagedTasks, _) = await _taskRepository.GetPagedAsync(UserId, pagedParams);

        var report = new TaskReportResponse
        {
            TotalTasks = tasks.Count,
            Status = BuildReport(tasks, tasks.Count, true),
            Priority = BuildReport(tasks, tasks.Count, false),
            Tasks = pagedTasks.Adapt<List<TaskResponse>>()
        };

        return ResultResponse<TaskReportResponse>.Success(report);
    }

    private List<ReportCategoryResponse> BuildReport(IEnumerable<TaskEntity> tasks, int totalTasks, bool isStatus)
    {
        var statusReport = tasks
            .Select(task => new { Id = task.StatusId, Name = task.TaskStatus.Name });

        var priorityReport = tasks
            .Select(task => new { Id = task.PriorityId, Name = task.TaskPriority.Name });

        var report = isStatus ? statusReport : priorityReport;

        return report
            .GroupBy(item => new { item.Id, item.Name })
            .Select(group => BuildReportCategory(group.Key.Id, group.Key.Name, group.Count(), totalTasks))
            .ToList();
    }

    private ReportCategoryResponse BuildReportCategory(int id, string name, int count, int totalTasks)
    {
        return new ReportCategoryResponse
        {
            Id = id,
            Name = name,
            Count = count,
            Percentage = totalTasks == 0 ? 0 : Math.Round((decimal)count / totalTasks * 100, 2)
        };
    }


}


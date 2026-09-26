using FluentValidation;
using Mapster;
using TaskManager.Application.DTOs;
using TaskManager.Application.DTOs.Task.Report;
using TaskManager.Application.DTOs.Task.Request;
using TaskManager.Application.DTOs.Task.Response;
using TaskManager.Application.Interfaces;
using TaskManager.Application.Validators;
using TaskManager.Application.Validators.Task;
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
        if (taskId == Guid.Empty)
            return ResultResponse<TaskResponse>.Failure(string.Format(Messages.RESOURCE_NOT_FOUND, "a tarefa"));

        TaskEntity? task = await _taskRepository.GetTaskByIdAsync(taskId, UserId);

        if (task == null)
            return ResultResponse<TaskResponse>.Failure(string.Format(Messages.RESOURCE_NOT_FOUND, "a tarefa"));

        TaskResponse taskDto = task.Adapt<TaskResponse>();

        return ResultResponse<TaskResponse>.Success(taskDto);
    }

    public async Task<ResultResponse<PagedResultDto<TaskResponse>>> GetPagedAsync(TaskPagedParams pagedParams)
    {
        TaskPagedParamsValidator validator = new TaskPagedParamsValidator();
        await validator.ValidateAndThrowAsync(pagedParams);

        var (tasks, totalCount) = await _taskRepository.GetPagedAsync(UserId, pagedParams);

        List<TaskResponse> responses = tasks.Adapt<List<TaskResponse>>();

        PagedResultDto<TaskResponse> pagedResult = new PagedResultDto<TaskResponse>(responses, pagedParams.PageNumber, pagedParams.PageSize, totalCount);

        return ResultResponse<PagedResultDto<TaskResponse>>.Success(pagedResult);
    }

    public async Task<ResultResponse<string>> CreateTaskAsync(CreateTaskRequest request)
    {
        CreateTaskValidator validator = new CreateTaskValidator();
        await validator.ValidateAndThrowAsync(request);

        TaskEntity newTask = request.Adapt<TaskEntity>();
        newTask.UserId = UserId;

        await _taskRepository.CreateTaskAsync(newTask);

        return ResultResponse<string>.Success(string.Format(Messages.OPERATION_SUCCESS), "Tarefa criada");
    }
    public async Task<ResultResponse<string>> EditTaskAsync(EditTaskRequest request)
    {
        EditTaskValidator validator = new EditTaskValidator();
        await validator.ValidateAndThrowAsync(request);

        TaskEntity? task = await _taskRepository.GetTaskByIdAsync(request.Id, UserId);

        if (task == null)
            return ResultResponse<string>.Failure(string.Format(Messages.RESOURCE_NOT_FOUND, "a tarefa"));

        request.Adapt(task);

        await _taskRepository.EditTaskAsync(task);

        return ResultResponse<string>.Success(Messages.OPERATION_SUCCESS, "Tarefa atualizada");
    }

    public async Task<ResultResponse<string>> DeleteTaskAsync(DeleteTaskRequest request)
    {
        DeleteTaskValidator validator = new DeleteTaskValidator();
        await validator.ValidateAndThrowAsync(request);

        List<TaskEntity> deletedTasks = [];

        foreach (Guid taskId in request.TaskId)
        {
            TaskEntity? task = await _taskRepository.GetTaskByIdAsync(taskId, UserId);

            if (task == null) 
                continue;

            await _taskRepository.DeleteTaskAsync(task);
            deletedTasks.Add(task);
        }

        if (deletedTasks.Count == 0)
            return ResultResponse<string>.Failure(string.Format(Messages.RESOURCE_NOT_FOUND, "as tarefas"));

        int notFoundCount = request.TaskId!.Count - deletedTasks.Count;

        return ResultResponse<string>.Success(string.Format(Messages.TASKS_DELETED_SUCCESSFULLY,deletedTasks.Count));
    }

    public async Task<ResultResponse<TaskSelectablesResponse>> GetSelectablesAsync()
    {
        List<Core.Entities.TaskStatus> statuses = await _taskRepository.GetTaskStatusesAsync();
        List<TaskPriority> priorities = await _taskRepository.GetTaskPrioritiesAsync();

        TaskSelectablesResponse response = new TaskSelectablesResponse
        {
            Status = statuses,
            Priority = priorities
        };

        return ResultResponse<TaskSelectablesResponse>.Success(response);
    }

    public async Task<ResultResponse<TaskReportResponse>> GetReportAsync(ReportPagedParams reportParams)
    {
        ReportPagedParamsValidator validator = new ReportPagedParamsValidator();
        await validator.ValidateAndThrowAsync(reportParams);

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


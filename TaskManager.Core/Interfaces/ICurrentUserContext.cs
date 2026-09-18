namespace TaskManager.Core.Interfaces;

public interface ICurrentUserContext
{
    Guid UserId { get; }
}

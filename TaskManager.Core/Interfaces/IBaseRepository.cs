

namespace TaskManager.Core.Interfaces;

public interface IBaseRepository
{
    IQueryable<T> ApplySort<T>(IQueryable<T> query, string sort, string order);
}

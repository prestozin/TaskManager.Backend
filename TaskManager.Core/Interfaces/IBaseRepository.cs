

namespace TaskManager.Core.Interfaces;

public interface IBaseRepository<T> where T : class
{
    IQueryable<T> ApplySort(IQueryable<T> query, string sort, string order);
}

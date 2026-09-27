using TaskManager.Infra.Data.Repositories;

namespace TaskManager.Tests.Repositories;

public class BaseRepositoryTests
{
    private sealed class TestEntity
    {
        public string Name { get; set; } = string.Empty;
        public int Priority { get; set; }
    }

    [Fact]
    public void ShouldSortAscending_WhenOrderIsAsc()
    {
        BaseRepository<TestEntity> repository = new BaseRepository<TestEntity>();

        IQueryable<TestEntity> query = new List<TestEntity>
        {
            new TestEntity { Name = "C", Priority = 3 },
            new TestEntity { Name = "A", Priority = 1 },
            new TestEntity { Name = "B", Priority = 2 }
        }.AsQueryable();

        var result = repository.ApplySort(query, nameof(TestEntity.Priority), "asc").ToList();

        Assert.Equal([1, 2, 3], result.Select(item => item.Priority));
    }

    [Fact]
    public void ShouldSortDescending_WhenOrderIsDesc()
    {
        BaseRepository<TestEntity> repository = new BaseRepository<TestEntity>();

        IQueryable<TestEntity> query = new List<TestEntity>
        {
            new TestEntity { Name = "A", Priority = 1 },
            new TestEntity { Name = "C", Priority = 3 },
            new TestEntity { Name = "B", Priority = 2 }
        }.AsQueryable();

        var result = repository.ApplySort(query, nameof(TestEntity.Priority), "desc").ToList();

        Assert.Equal([3, 2, 1], result.Select(item => item.Priority));
    }

    [Fact]
    public void ShouldKeepQueryOrder_WhenSortPropertyDoesNotExist()
    {
        BaseRepository<TestEntity> repository = new BaseRepository<TestEntity>();

        IQueryable<TestEntity> query = new List<TestEntity>
        {
            new TestEntity { Name = "B", Priority = 2 },
            new TestEntity { Name = "A", Priority = 1 }
        }.AsQueryable();

        var result = repository.ApplySort(query, "Unknown", "asc").ToList();

        Assert.Equal(["B", "A"], result.Select(item => item.Name));
    }
}

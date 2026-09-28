using TaskManager.Infra.Data.Repositories;

namespace TaskManager.Tests.Repositories;

[FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
public class BaseRepositoryTests
{
    private sealed class TestEntity
    {
        public string Name { get; set; } = string.Empty;
        public int Priority { get; set; }
    }

    [Test]
    public void ShouldSortAscending_WhenOrderIsAsc()
    {
        // Arrange
        BaseRepository<TestEntity> repository = new BaseRepository<TestEntity>();

        IQueryable<TestEntity> query = new List<TestEntity>
        {
            new TestEntity { Name = "C", Priority = 3 },
            new TestEntity { Name = "A", Priority = 1 },
            new TestEntity { Name = "B", Priority = 2 }
        }.AsQueryable();

        // Act
        var result = repository.ApplySort(query, nameof(TestEntity.Priority), "asc").ToList();

        // Assert
        Assert.That(result.Select(item => item.Priority), Is.EqualTo(new[] { 1, 2, 3 }));
    }

    [Test]
    public void ShouldSortDescending_WhenOrderIsDesc()
    {
        // Arrange
        BaseRepository<TestEntity> repository = new BaseRepository<TestEntity>();

        IQueryable<TestEntity> query = new List<TestEntity>
        {
            new TestEntity { Name = "A", Priority = 1 },
            new TestEntity { Name = "C", Priority = 3 },
            new TestEntity { Name = "B", Priority = 2 }
        }.AsQueryable();

        // Act
        var result = repository.ApplySort(query, nameof(TestEntity.Priority), "desc").ToList();

        // Assert
        Assert.That(result.Select(item => item.Priority), Is.EqualTo(new[] { 3, 2, 1 }));
    }

    [Test]
    public void ShouldKeepQueryOrder_WhenSortPropertyDoesNotExist()
    {
        // Arrange
        BaseRepository<TestEntity> repository = new BaseRepository<TestEntity>();

        IQueryable<TestEntity> query = new List<TestEntity>
        {
            new TestEntity { Name = "B", Priority = 2 },
            new TestEntity { Name = "A", Priority = 1 }
        }.AsQueryable();

        // Act
        var result = repository.ApplySort(query, "Unknown", "asc").ToList();

        // Assert
        Assert.That(result.Select(item => item.Name), Is.EqualTo(new[] { "B", "A" }));
    }
}

using TaskManager.Core.Shared;

namespace TaskManager.Tests.Shared;

[FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
public class PagedResultDtoTests
{
    [Test]
    public void ShouldCalculateTotalPages_WhenTotalCountIsNotMultipleOfPageSize()
    {
        // Arrange

        // Act
        PagedResultDto<int> result = new PagedResultDto<int>([1, 2], 2, 10, 21);

        // Assert
        Assert.That(result.TotalPages, Is.EqualTo(3));
        Assert.That(result.PageNumber, Is.EqualTo(2));
        Assert.That(result.PageSize, Is.EqualTo(10));
        Assert.That(result.TotalCount, Is.EqualTo(21));
        Assert.That(result.Items, Is.EqualTo(new[] { 1, 2 }));
    }

    [Test]
    public void ShouldAllowPropertyAssignment_WhenDefaultConstructorIsUsed()
    {
        // Arrange

        // Act
        PagedResultDto<int> result = new PagedResultDto<int>
        {
            Items = [1],
            PageNumber = 1,
            PageSize = 10,
            TotalCount = 1
        };

        // Assert
        Assert.That(result.TotalPages, Is.EqualTo(1));
    }
}

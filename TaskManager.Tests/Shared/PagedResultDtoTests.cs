using TaskManager.Core.Shared;

namespace TaskManager.Tests.Shared;

public class PagedResultDtoTests
{
    [Fact]
    public void ShouldCalculateTotalPages_WhenTotalCountIsNotMultipleOfPageSize()
    {
        PagedResultDto<int> result = new PagedResultDto<int>([1, 2], 2, 10, 21);

        Assert.Equal(3, result.TotalPages);
        Assert.Equal(2, result.PageNumber);
        Assert.Equal(10, result.PageSize);
        Assert.Equal(21, result.TotalCount);
        Assert.Equal([1, 2], result.Items);
    }

    [Fact]
    public void ShouldAllowPropertyAssignment_WhenDefaultConstructorIsUsed()
    {
        PagedResultDto<int> result = new PagedResultDto<int>
        {
            Items = [1],
            PageNumber = 1,
            PageSize = 10,
            TotalCount = 1
        };

        Assert.Equal(1, result.TotalPages);
    }
}

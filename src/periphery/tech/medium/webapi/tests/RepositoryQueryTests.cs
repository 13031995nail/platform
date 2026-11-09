namespace Tmobis.Medium.WebApi.Tests;

using System.ComponentModel.DataAnnotations.Schema;
using Tmobis.Core.Converters;
using Tmobis.Core.Entities;
using Tmobis.Core.Models;
using Xunit;

public sealed class SortingExpressionBuilderTests
{
    private readonly SortingExpressionBuilder builder =
        new(new PropertyNameConverter());

    [Fact]
    public void EmptySortingDefaultsToDescendingId()
    {
        var items = new[]
        {
            new SortableEntity { Id = Guid.Parse("00000000-0000-0000-0000-000000000001") },
            new SortableEntity { Id = Guid.Parse("00000000-0000-0000-0000-000000000002") }
        };

        var sorted = builder.Build<SortableEntity>(null)(items.AsQueryable()).ToList();
        Assert.Equal(items[1].Id, sorted[0].Id);
    }

    [Theory]
    [InlineData("name", false)]
    [InlineData("-name", true)]
    public void ScalarSortingWorks(string field, bool descending)
    {
        var items = new[]
        {
            new SortableEntity { Name = "B" },
            new SortableEntity { Name = "A" }
        };

        var sorted = builder.Build<SortableEntity>(field)(items.AsQueryable()).ToList();
        Assert.Equal(descending ? "B" : "A", sorted[0].Name);
    }

    [Theory]
    [InlineData("-")]
    [InlineData("unknown")]
    [InlineData("children")]
    [InlineData("computed")]
    public void InvalidSortingIsRejectedBeforeExecutingQuery(string sorting)
        => Assert.Throws<ArgumentException>(() => builder.Build<SortableEntity>(sorting));

    private sealed class SortableEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = "";
        public List<string> Children { get; set; } = [];
        [NotMapped]
        public string Computed { get; set; } = "";
    }
}

public sealed class EntityListQueryHelperTests
{
    [Fact]
    public async Task ExcessivelyLargePageIsRejectedBeforeQuery()
    {
        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() =>
            EntityListQueryHelper.GetList<IEntity>(
                null!, int.MaxValue, 20, 10, 100));
    }
}

using Application.Features.Solicitudes;
using FluentAssertions;

namespace Application.Tests;

public sealed class IbOncoPaginationTests
{
    [Fact]
    public void Normalize_UsesPageWhenOffsetIsNotProvided()
    {
        var result = IbOncoPagination.Normalize(page: 2, limit: 100, offset: null);

        result.Should().Be((2, 100, 100));
    }

    [Fact]
    public void Normalize_AcceptsFirstPageWithoutOffset()
    {
        var result = IbOncoPagination.Normalize(page: 1, limit: 1000, offset: null);

        result.Should().Be((1, 1000, 0));
    }

    [Fact]
    public void Normalize_UsesExplicitOffsetAndCalculatesEffectivePage()
    {
        var result = IbOncoPagination.Normalize(page: 1, limit: 100, offset: 250);

        result.Should().Be((3, 100, 250));
    }

    [Fact]
    public void Normalize_ClampsLimitToSupportedRange()
    {
        IbOncoPagination.Normalize(null, 0, null).Limit.Should().Be(1);
        IbOncoPagination.Normalize(null, 5000, null).Limit.Should().Be(1000);
    }
}

using FluentAssertions;
using Xunit;

namespace M0glan.Functional.Test;

public class ObjectExtensionsTest
{
    [Fact]
    public void Id()
    {
        const string s = "text";
        s.Id().Should().Be(s);
        
        var okResult = Result.Ok<int, string>(3);
        okResult.Id().Should().Be(okResult);
        
        var errorResult = Result.Error<int, string>("Something went wrong.");
        errorResult.Id().Should().Be(errorResult);
    }

    [Fact]
    public void Ignore()
    {
        "text".Ignore().Should().Be(Unit.Instance);
        3.Ignore().Should().Be(Unit.Instance);
    }
}
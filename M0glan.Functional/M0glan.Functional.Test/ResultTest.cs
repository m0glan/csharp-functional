using System;
using FluentAssertions;
using Xunit;

namespace M0glan.Functional.Test;

public class ResultTest
{
    [Fact]
    public void Ok()
    {
        var okResult = Result.Ok<int, string>(3);
        okResult.IsError.Should().BeFalse();
    }

    [Fact]
    public void Error()
    {
        var errorResult = Result.Error<int, string>("Something went wrong...");
        errorResult.IsError.Should().BeTrue();
    }
    
    [Fact]
    public void Match()
    {
        var okResult = Result.Ok<int, string>(3);
        okResult.Match<Unit>(x =>
        {
            x.Should().Be(3);
            return Unit.Instance;
        }, e =>
        {
            Assert.Fail($"Should not have fallen into error path with error '{e}'.");
            return Unit.Instance;
        });

        const string expectedError = "It's an error";
        var errorResult = Result.Error<int, string>(expectedError);
        errorResult.Match<Unit>(x =>
        {
            Assert.Fail($"Should not have fallen into ok path with result '{x}'.");
            return Unit.Instance;
        }, e =>
        {
            e.Should().BeEquivalentTo(expectedError);
            return Unit.Instance;
        });
    }
    
    [Fact]
    public void Bind()
    {
        const string expected = "The result of 3 + 2 is 5.";
        var okResult = 
            Result.Ok<int, string>(3)
                .Bind(x => Result.Ok<int, string>(x + 2))
                .Bind(x => Result.Ok<string, string>($"The result of 3 + 2 is {x}."))
                .Bind(actual =>
                {
                    actual.Should().BeEquivalentTo(expected);
                    return Result.Ok<Unit, string>(Unit.Instance);
                });
        Assert.False(okResult.IsError);
        
        var errorResult = 
            Result.Ok<int, string>(3)
                .Bind(x => Result.Ok<int, string>(x + 2))
                .Bind(_ => Result.Error<string, string>($"Something went wrong."))
                .Bind(_ =>
                {
                    Assert.Fail("Should not bind a failed result.");
                    return Result.Ok<Unit, string>(Unit.Instance);
                });
        errorResult.IsError.Should().BeTrue();
    }

    [Fact]
    public void Map()
    {
        const string expected = "The result of 3 + 2 is 5.";
        var okResult = 
            Result.Ok<int, string>(3)
                .Map(x => x + 2)
                .Map(x => $"The result of 3 + 2 is {x}.")
                .Map(actual =>
                {
                    actual.Should().BeEquivalentTo(expected);
                    return Unit.Instance;
                });
        Assert.False(okResult.IsError);
        
        var errorResult = 
            Result.Ok<int, string>(3)
                .Map(x => x + 2)
                .Bind(_ => Result.Error<string, string>($"Something went wrong."))
                .Map(_ =>
                {
                    Assert.Fail("Should not map a failed result.");
                    return Unit.Instance;
                });
        errorResult.IsError.Should().BeTrue();
    }
    
    [Fact]
    public void MapError()
    {
        const string errorMessage = "Something went wrong.";

        Result.Error<string, Exception>(new Exception(errorMessage))
            .MapError(e =>
            {
                e.Should().BeOfType<Exception>();
                e.Message.Should().BeEquivalentTo(errorMessage);
                return e.Message;
            })
            .MapError(e =>
            {
                e.Should().BeOfType<string>();
                e.Should().BeEquivalentTo(errorMessage);
                return e;
            }).IsError.Should().BeTrue();
    }

    [Fact]
    public void Ignore()
    {
        var ignoredResult =
            Result.Ok<int, string>(3)
                .Ignore()
                .Bind(u =>
                {
                    u.Should().BeOfType<Unit>();
                    return Result.Ok<Unit, string>(Unit.Instance);
                });
        ignoredResult.IsError.Should().BeFalse();

        ignoredResult =
            Result.Error<int, string>("Something went wrong.")
                .Ignore()
                .Bind(u =>
                {
                    Assert.Fail("Should not bind a failed result");
                    return Result.Ok<Unit, string>(Unit.Instance);
                });
        ignoredResult.IsError.Should().BeTrue();
    }
}
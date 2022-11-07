namespace BaseLibrary.Tests;
public class LogicalResultTests
{
    [Fact(DisplayName = "Create Result with data as default Failure")]
    public void CreateResult_AsDefault()
    {
        Result result = default;

        Assert.False(result.IsSuccess);
        Assert.False(result);
        Assert.True(result.IsFailure);
    }

    [Fact(DisplayName = "Create success Result")]
    public void CreateResult_AsSuccess()
    {
        var result = Results.OnSuccess();

        Assert.True(result.IsSuccess);
        Assert.True(result);
        Assert.False(result.IsFailure);
        Assert.False(result.IsException);
    }

    [Fact(DisplayName = "Create Result as failure")]
    public void CreateResult_AsFailure()
    {
        var result = Results.OnFailure();

        Assert.False(result.IsSuccess);
        Assert.False(result);
        Assert.True(result.IsFailure);
        Assert.False(result.IsException);
    }

    [Fact(DisplayName = "Create Result as Exception")]
    public void CreateResult_AsException()
    {
        var result = Results.OnException(new Exception());

        Assert.False(result.IsSuccess);
        Assert.True(result.IsException);
        Assert.False(result.IsFailure);
        Assert.False(result);
        Assert.NotNull(result.Exception);
    }

    #region BLACK MAGIC

    [Fact(DisplayName = "Bind Result with success")]
    public void BindResult_AsSuccess()
    {
        var result =
            Results.OnSuccess(42)
                .Bind(x => Results.OnSuccess(x + 1))
                .Bind(x => Results.OnSuccess(x + 2))
                .Bind(x => Results.OnSuccess(x - 2));

        Assert.True(result.IsSuccess);
        Assert.False(result.IsException);
        Assert.False(result.IsFailure);
        Assert.True(result);
        Assert.Equal(43, result.Data);
    }

    [Fact(DisplayName = "Bind Result with failure")]
    public void BindResult_AsSuccess_WithFailure()
    {
        var result =
            Results.OnSuccess(42)
                .Bind(x => Results.OnSuccess(x + 1))
                .Bind(x => Results.OnFailure<int>("OP failed"))
                .Bind(x => Results.OnSuccess(x - 2));

        Assert.False(result.IsSuccess);
        Assert.False(result.IsException);
        Assert.True(result.IsFailure);
        Assert.False(result);
        Assert.Equal("OP failed", result.Message);
    }

    [Fact(DisplayName = "Bind Result with exception")]
    public void BindResult_AsSuccess_WithException()
    {
        var result =
            Results.OnSuccess(42)
                .Bind(x => Results.OnSuccess(x + 1))
                .Bind(x => Results.OnException<int>(new Exception()))
                .Bind(x => Results.OnSuccess(x - 2));

        Assert.False(result.IsSuccess);
        Assert.True(result.IsException);
        Assert.False(result.IsFailure);
        Assert.False(result);
    }

    [Fact(DisplayName = "Bind async Result with success")]
    public async void BindResult_AsAsyncSuccess()
    {
        var result =
            await Task.FromResult(Results.OnSuccess(42))
                .Bind(x => Task.FromResult(Results.OnSuccess(x + 1)))
                .Bind(x => Task.FromResult(Results.OnSuccess(x + 2)))
                .Bind(x => Task.FromResult(Results.OnSuccess(x - 2)));

        Assert.True(result.IsSuccess);
        Assert.False(result.IsException);
        Assert.False(result.IsFailure);
        Assert.True(result);
        Assert.Equal(43, result.Data);
    }

    [Fact(DisplayName = "Bind async Result with failure")]
    public async void BindResult_AsAsyncSuccess_WithFailure()
    {
        var result =
            await Task.FromResult(Results.OnSuccess(42))
                .Bind(x => Task.FromResult(Results.OnSuccess(x + 1)))
                .Bind(x => Task.FromResult(Results.OnFailure<int>("OP failed")))
                .Bind(x => Task.FromResult(Results.OnSuccess(x - 2)));

        Assert.False(result.IsSuccess);
        Assert.False(result.IsException);
        Assert.True(result.IsFailure);
        Assert.False(result);
        Assert.Equal("OP failed", result.Message);
    }

    [Fact(DisplayName = "Bind async Result with exception")]
    public async void BindResult_AsAsyncSuccess_WithException()
    {
        var result =
            await Task.FromResult(Results.OnSuccess(42))
                .Bind(x => Task.FromResult(Results.OnSuccess(x + 1)))
                .Bind(x => Task.FromResult(Results.OnException<int>(new Exception())))
                .Bind(x => Task.FromResult(Results.OnSuccess(x - 2)));

        Assert.False(result.IsSuccess);
        Assert.True(result.IsException);
        Assert.False(result.IsFailure);
        Assert.False(result);
    }

    [Fact(DisplayName = "Bind Result over Result[T] and vice-versa")]
    public void BindResult_AsSuccess_WithResult()
    {
        var result =
            Results.OnSuccess(42)
                .Bind(x => Results.OnSuccess(x + 1))
                .Bind(x => Results.OnSuccess())
                .Bind(() => Results.OnSuccess(2));

        Assert.True(result.IsSuccess);
        Assert.False(result.IsException);
        Assert.False(result.IsFailure);
        Assert.True(result);
        Assert.Equal(2, result.Data);
    }

    [Fact(DisplayName = "Bind async Result over Result[T] and vice-versa")]
    public async void BindResult_AsAsyncSuccess_WithResult()
    {
        var result =
            await Task.FromResult(Results.OnSuccess(42))
                .Bind(x => Task.FromResult(Results.OnSuccess(x + 1)))
                .Bind(x => Task.FromResult(Results.OnSuccess()))
                .Bind(() => Task.FromResult(Results.OnSuccess(2)));

        Assert.True(result.IsSuccess);
        Assert.False(result.IsException);
        Assert.False(result.IsFailure);
        Assert.True(result);
        Assert.Equal(2, result.Data);
    }
    #endregion
}

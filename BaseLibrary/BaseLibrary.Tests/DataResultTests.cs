namespace BaseLibrary.Tests;
public class DataResultTests
{
    [Fact(DisplayName = "Create Result with integer data")]
    public void CreateResult_AsSuccessInteger()
    {
        int data = 42;
        var result = Results.OnSuccess(data);

        Assert.True(result.IsSuccess);
        Assert.True(result);
        Assert.False(result.IsFailure);
        Assert.False(result.IsException);
        Assert.Equal(data, result.Data);
    }


    [Fact(DisplayName = "Create Result with reference type data")]
    public void CreateResult_AsSuccessObject()
    {
        var data = new { FirstWord = "Hello", LastWord = "there" };
        var result = Results.OnSuccess(data);

        Assert.True(result.IsSuccess);
        Assert.True(result);
        Assert.False(result.IsFailure);
        Assert.False(result.IsException);
        Assert.Equal(data.FirstWord, result.Data.FirstWord);
        Assert.Equal(data.LastWord, result.Data.LastWord);
    }

    [Fact(DisplayName = "Create Result with value type data")]
    public void CreateResult_AsSuccessValueType()
    {
        var data = (FirstWord: "Hello", LastWord: "there");
        var result = Results.OnSuccess(data);

        Assert.True(result.IsSuccess);
        Assert.True(result);
        Assert.False(result.IsFailure);
        Assert.False(result.IsException);
        Assert.Equal(data.FirstWord, result.Data.FirstWord);
        Assert.Equal(data.LastWord, result.Data.LastWord);
    }

    [Fact(DisplayName = "Create Result as failure with reference type")]
    public void CreateResult_AsFailure_WithObject()
    {
        var result = Results.OnFailure<object>();

        Assert.False(result.IsSuccess);
        Assert.False(result);
        Assert.True(result.IsFailure);
        Assert.False(result.IsException);
        Assert.Null(result.Data);
    }

    [Fact(DisplayName = "Create Result as failure with integer")]
    public void CreateResult_AsNone_WithInteger()
    {
        var result = Results.OnFailure<int>();

        Assert.False(result.IsSuccess);
        Assert.False(result.IsException);
        Assert.False(result);
        Assert.True(result.IsFailure);
        Assert.Equal(default, result.Data);
    }

    [Fact(DisplayName = "Create Result as Success with null data")]
    public void CreateResult_AsSuccess_WithNullData()
    {
        var result = Results.OnSuccess<object>(null);

        Assert.False(result.IsSuccess);
        Assert.False(result.IsException);
        Assert.False(result);
        Assert.True(result.IsFailure);
        Assert.Null(result.Data);
    }

    [Fact(DisplayName = "Create Result as Exception")]
    public void CreateResult_AsException()
    {
        var result = Results.OnException<object>(new Exception());

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
    #endregion
}

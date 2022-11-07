using BaseLibrary;

namespace BaseLibrary.Tests;

public class OptionsTests
{

    [Fact(DisplayName = "Create Option as default None")]
    public void CreateOption_AsDefault()
    {
        Option<object> objectOption = default;
        Option<int> valueOption = default;

        Assert.False(objectOption.IsSome);
        Assert.False(objectOption);
        Assert.True(objectOption.IsNone);
        Assert.False(valueOption.IsSome);
        Assert.False(valueOption);
        Assert.True(valueOption.IsNone);
    }

    [Fact(DisplayName = "Create Option with integer data")]
    public void CreateOption_AsSomeInteger()
    {
        int data = 42;
        var option = Options.Some(data);

        Assert.True(option.IsSome);
        Assert.True(option);
        Assert.False(option.IsNone);
        Assert.Equal(data, option.Data);
    }


    [Fact(DisplayName = "Create Option with reference type data")]
    public void CreateOption_AsSomeObject()
    {
        var data = new {FirstWord = "Hello", LastWord = "there"};
        var option = Options.Some(data);

        Assert.True(option.IsSome);
        Assert.True(option);
        Assert.False(option.IsNone);
        Assert.Equal(data.FirstWord, option.Data.FirstWord);
        Assert.Equal(data.LastWord, option.Data.LastWord);
    }

    [Fact(DisplayName = "Create Option with value type data")]
    public void CreateOption_AsSomeValueType()
    {
        var data = (FirstWord: "Hello", LastWord:"there");
        var option = Options.Some(data);

        Assert.True(option.IsSome);
        Assert.True(option);
        Assert.False(option.IsNone);
        Assert.Equal(data.FirstWord, option.Data.FirstWord);
        Assert.Equal(data.LastWord, option.Data.LastWord);
    }

    [Fact(DisplayName = "Create Option as None with reference type")]
    public void CreateOption_AsNone_WithObject()
    {
        // some type has to be given - None is an alternative
        var option = Options.None<object>();

        Assert.False(option.IsSome);
        Assert.False(option);
        Assert.True(option.IsNone);
        Assert.Null(option.Data);
    }

    [Fact(DisplayName = "Create Option as None with integer")]
    public void CreateOption_AsNone_WithInteger()
    {
        // some type has to be given - None is an alternative
        var option = Options.None<int>();

        Assert.False(option.IsSome);
        Assert.False(option);
        Assert.True(option.IsNone);
        Assert.Equal(default, option.Data);
    }

    [Fact(DisplayName = "Create Option as Some with null data")]
    public void CreateOption_AsSome_WithNullData()
    {
        var option = Options.Some<object>(null);

        Assert.False(option.IsSome);
        Assert.False(option);
        Assert.True(option.IsNone);
        Assert.Null(option.Data);
    }

    #region BLACK MAGIC

    [Fact(DisplayName = "Map a Some Option on integer")]
    public void MapOption_AsSome_WithInteger_ToString()
    {
        var option = Options.Some(42);

        var mappedOption =
            option.Map(x => x + 1)
                  .Map(x => x - 2)
                  .Map(x => x + 2)
                  .Map(x => x.ToString());

        Assert.True(mappedOption.IsSome);
        Assert.True(mappedOption);
        Assert.False(mappedOption.IsNone);
        Assert.Equal("43", mappedOption.Data);
    }

    [Fact(DisplayName = "Map a Some Option on a reference type")]
    public void MapOption_AsSome_WithObject()
    {
        var data = new { FirstWord = "Hello", LastWord = "there"};
        var expectedData = new { FirstWord = "Hello", MiddleWord = "there", LastWord = "over"};
        var option = Options.Some(data);

        var mappedOption =
            option.Map(x => new { FirstWord = x.FirstWord, MiddleWord = "over", LastWord = x.LastWord })
                  .Map(x => new { FirstWord = x.MiddleWord, LastWord = x.LastWord })
                  .Map(x => new { FirstWord = "Hello", MiddleWord = x.LastWord, LastWord = x.FirstWord });

        Assert.True(mappedOption.IsSome);
        Assert.True(mappedOption);
        Assert.False(mappedOption.IsNone);
        Assert.Equal(expectedData.FirstWord, mappedOption.Data.FirstWord);
        Assert.Equal(expectedData.MiddleWord, mappedOption.Data.MiddleWord);
        Assert.Equal(expectedData.LastWord, mappedOption.Data.LastWord);
    }

    [Fact(DisplayName = "Map a None Option on integer")]
    public void MapOption_AsNone_WithInteger()
    {
        var option = Options.None<int>();

        var mappedOption =
            option.Map(x => x + 1)
                  .Map<int, int>(x => x + 2);

        Assert.False(mappedOption.IsSome);
        Assert.False(mappedOption);
        Assert.True(mappedOption.IsNone);
    }

    [Fact(DisplayName = "Bind a Some Option on integer")]
    public void BindOption_AsSome_WithInteger()
    {
        var option = Options.Some(42);

        var bindedOption =
            option.Bind(x => Options.Some(x + 1))
                  .Bind(x => Options.Some(x + 2))
                  .Bind(x => Options.Some(x - 2))
                  .Bind(x => Options.Some(x.ToString()));

        Assert.True(bindedOption.IsSome);
        Assert.True(bindedOption);
        Assert.False(bindedOption.IsNone);
        Assert.Equal("43", bindedOption.Data);
    }

    [Fact(DisplayName = "Bind a Some Option on integer")]
    public void BindOption_AsSome_WithInteger_ToNone()
    {
        var option = Options.Some(42);

        var bindedOption =
            option.Bind(x => Options.Some(x + 1))
                  .Bind(x => Options.Some(x + 2))
                  .Bind(x => Options.None<int>())
                  .Bind(x => Options.Some(x.ToString()));

        Assert.False(bindedOption.IsSome);
        Assert.False(bindedOption);
        Assert.True(bindedOption.IsNone);
    }

    #endregion
}
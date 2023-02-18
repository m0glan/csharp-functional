namespace Vladmo.Functional;

public static class Result
{
    public static Result<TOk, TError> Ok<TOk, TError>(TOk value) => new(value);

    public static Result<TOk, TError> Error<TOk, TError> (TError error) => new(error);
}

public class Result<TOk, TError>
{
    private readonly TOk? _ok;
    private readonly TError? _error;

    public bool IsError => _error is not null;

    internal Result(TOk ok)
    {
        _ok = ok;
    }

    internal Result(TError error)
    {
        _error = error;
    }

    public T Match<T>(Func<TOk, T> onOk, Func<TError, T> onFail)
    {
        if (_error is not null)
        {
            return onFail(_error);
        }

        return _ok is not null ? onOk(_ok) : default!;
    }

    public Result<TOk2, TError> Bind<TOk2>(Func<TOk, Result<TOk2, TError>> binder)
    {
        return Match(binder, Result.Error<TOk2, TError>);
    }

    public Result<TOk2, TError> Map<TOk2>(Func<TOk, TOk2> mapping)
    {
        return Bind(x => Result.Ok<TOk2, TError>(mapping(x)));
    }

    public Result<TOk, TError2> MapError<TError2>(Func<TError, TError2> mapping)
    {
        return Match(Result.Ok<TOk, TError2>, e => Result.Error<TOk, TError2>(mapping(e)));
    }

    public Result<Unit, TError> Ignore() => Map(x => x!.Ignore());
}
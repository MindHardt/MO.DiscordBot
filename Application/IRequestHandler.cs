namespace Application;

public interface IRequestHandler<in TRequest, TResult>
{
    /// <summary>
    /// Handles <paramref name="request"/>, producing <typeparamref name="TResult"/>.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    public Task<TResult> HandleAsync(TRequest request, CancellationToken ct = default);
}

public interface IRequestHandler<in TRequest>
{
    /// <summary>
    /// Handles <paramref name="request"/>.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    public Task HandleAsync(TRequest request, CancellationToken ct = default);
}
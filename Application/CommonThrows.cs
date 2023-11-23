using System.Diagnostics.CodeAnalysis;

namespace Application;

public static class CommonThrows
{
    /// <inheritdoc cref="CommonResources.AccessDeniedErrorMessage"/>
    [DoesNotReturn]
    public static void ThrowAccessDenied(string action)
        => throw new ArgumentException(string.Format(CommonResources.AccessDeniedErrorMessage, action));
}
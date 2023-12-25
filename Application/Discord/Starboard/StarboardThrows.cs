using System.Diagnostics.CodeAnalysis;

namespace Application.Discord.Starboard;

public static class StarboardThrows
{
    /// <inheritdoc cref="StarboardResources.TrackExistsErrorMessage"/>
    [DoesNotReturn]
    public static void ThrowTrackExists(string emoji)
        => throw new InvalidOperationException(string.Format(StarboardResources.TrackExistsErrorMessage, emoji));

    /// <inheritdoc cref="StarboardResources.TrackExistsErrorMessage"/>
    [DoesNotReturn]
    public static void ThrowTrackNotFound(string emoji)
        => throw new InvalidOperationException(string.Format(StarboardResources.TrackNotFoundErrorMessage, emoji));
    
    /// <inheritdoc cref="StarboardResources.TrackExistsErrorMessage"/>
    [DoesNotReturn]
    public static void ThrowStarboardChannelNotAssigned()
        => throw new InvalidOperationException(StarboardResources.StarboardChannelNotAssignedErrorMessage);
}
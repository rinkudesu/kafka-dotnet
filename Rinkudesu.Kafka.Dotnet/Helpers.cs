using System.Diagnostics.CodeAnalysis;

namespace Rinkudesu.Kafka.Dotnet;

internal static class Helpers
{
    /// <summary>
    /// Tries do dispose an object using <paramref name="disposeAction"/> and handles <see cref="ObjectDisposedException"/>.
    /// </summary>
    /// <param name="disposing">Object to be disposed using <paramref name="disposeAction"/></param>
    /// <param name="disposeAction">Action that disposes of an object, that might throw <see cref="ObjectDisposedException"/>.</param>
    [ExcludeFromCodeCoverage]
    internal static void EnsureDisposed<T>(T disposing, Action<T> disposeAction)
    {
        try
        {
            disposeAction(disposing);
        }
        catch (ObjectDisposedException)
        {
            // this means that this object has already been disposed, which is fine
        }
    }
}

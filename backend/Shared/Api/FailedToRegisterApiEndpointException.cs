namespace Shared.Api;

public sealed class FailedToRegisterApiEndpointException(Type endpointType)
    : InvalidOperationException(string.Format(MessageFormat, endpointType.FullName))
{
    public const string MessageFormat = "Failed to register an API endpoint for type '{0}'.";
}

namespace Shared.Configuration;

public sealed class FailedToLoadConfigurationValueException(string section)
    : InvalidOperationException(string.Format(MessageFormat, section))
{
    public const string MessageFormat = "Failed to load '{0}' section value from configuration.";
}

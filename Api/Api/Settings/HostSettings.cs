namespace Api.Settings;

public record HostSettings
{
    public string Host { get; init; } = string.Empty;

    public IList<string> AllowedHosts
    {
        get => new[] { Host };
    }
}
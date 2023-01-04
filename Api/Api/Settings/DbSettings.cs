namespace Api.Settings;

public record DbSettings
{
    public string Host { get; init; } = string.Empty;

    public string Name { get; init; } = string.Empty;

    public string User { get; init; } = string.Empty;

    public string Password { get; init; } = string.Empty;

    public string ConnectionString
    {
        get => $"Server={Host};Port=5432;Database={Name};User Id={User};Password={Password};";
    }
}
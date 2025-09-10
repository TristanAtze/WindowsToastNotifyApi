namespace WindowsToastNotifyApi;

/// <summary>
/// Data passed back when the user activates (clicks) the toast or any of its buttons.
/// </summary>
public sealed class ToastActivationArgs
{
    public string Arguments { get; init; } = string.Empty;
    public IReadOnlyDictionary<string, string> Payload { get; init; } = new Dictionary<string, string>();
}

namespace Quotes;

public class Quote(string text, string author, string? url)
{
    public string Text { get; } = text;
    public string Author { get; } = author;
    public string? Url { get; } = url;
}

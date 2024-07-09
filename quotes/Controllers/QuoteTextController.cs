namespace Quotes.Controllers;

using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("[controller]")]
public class QuoteTextController : ControllerBase
{
    private readonly ILogger<QuoteController> _logger;
    private readonly IHttpClientFactory _httpClientFactory;

    public QuoteTextController(ILogger<QuoteController> logger, IHttpClientFactory httpClientFactory)
    {
        _logger = logger;
        _httpClientFactory = httpClientFactory;
    }

    [HttpGet(Name = "GetQuoteText")]
    public async Task<string> GetQuoteText([FromQuery] string? opsi)
    {
        _logger.LogInformation("At GetQuoteText");

        var activity = HttpContext.Features.Get<IHttpActivityFeature>()?.Activity;

        activity?.SetTag("x.foo", "bar");

        _logger.LogInformation("Current Activity Id={activityId} TraceId={traceId} SpanId={spanId}", activity?.Id, activity?.TraceId, activity?.SpanId);

        var requestUrl = string.IsNullOrEmpty(opsi) ? "quote" : $"quote?opsi={Uri.EscapeDataString(opsi)}";

        using var client = _httpClientFactory.CreateClient("quotes");

        var quote = await client.GetFromJsonAsync<Quote>(requestUrl);

        if (quote == null)
        {
            throw new ApplicationException("failed to get quote");
        }

        return $"{quote.Text} -- {quote.Author}";
    }
}

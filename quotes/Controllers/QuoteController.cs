namespace Quotes.Controllers;

using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("[controller]")]
public class QuoteController : ControllerBase
{
    private static readonly Quote[] Quotes =
    [
        new Quote
        (
            text: "To alcohol! The cause of... and solution to... all of life's problems.",
            author: "Homer Simpson", 
            url: "https://en.wikipedia.org/wiki/Homer_vs._the_Eighteenth_Amendment"
        ),
        new Quote
        (
            text: "You got to help me. I don't know what to do. I can't make decisions. I'm a president!",
            author: "President Skroob, Spaceballs", 
            url: "https://en.wikipedia.org/wiki/Spaceballs"
        ),
        new Quote
        (
            text: "Beware of he who would deny you access to information, for in his heart he dreams himself your master.",
            author: "Pravin Lal", 
            url: "https://alphacentauri.gamepedia.com/Peacekeeping_Forces"
        ),
        new Quote
        (
            text: "About the use of language: it is impossible to sharpen a pencil with a blunt axe. It is equally vain to try to do it with ten blunt axes instead.",
            author: "Edsger W. Dijkstra", 
            url: "https://www.cs.utexas.edu/users/EWD/transcriptions/EWD04xx/EWD498.html"
        ),
        new Quote
        (
            text: "Those hours of practice, and failure, are a necessary part of the learning process.",
            author: "Gina Sipley", 
            url: null
        ),
        new Quote
        (
            text: "Engineering is achieving function while avoiding failure.",
            author: "Henry Petroski", 
            url: null
        ),
        new Quote
        (
            text: "Leadership is defined by what you do, not what you're called.",
            author: "Jen Heemstra", 
            url: "https://twitter.com/jenheemstra/status/1260186699021287424"
        ),
        new Quote
        (
            text: "Don't only practice your art, but force your way into its secrets; art deserves that, for it and knowledge can raise man to the Divine.",
            author: "Ludwig van Beethoven", 
            url: null
        ),
    ];

    private readonly ILogger<QuoteController> _logger;

    public QuoteController(ILogger<QuoteController> logger)
    {
        _logger = logger;
    }

    [HttpGet(Name = "GetRandomQuote")]
    public Quote GetRandomQuote([FromQuery] string? opsi)
    {
        _logger.LogInformation("At GetRandomQuote");

        var activity = HttpContext.Features.Get<IHttpActivityFeature>()?.Activity;

        activity?.SetTag("x.foo", "bar");

        _logger.LogInformation("Current Activity Id={activityId} TraceId={traceId} SpanId={spanId}", activity?.Id, activity?.TraceId, activity?.SpanId);

        if (opsi != null)
        {
            throw new ApplicationException(opsi);
        }

        return Quotes[Random.Shared.Next(0, Quotes.Length)];
    }
}

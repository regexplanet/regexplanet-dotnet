using System.Text.Json;
using Microsoft.AspNetCore.HttpLogging;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHttpLogging(logging =>
{
    logging.LoggingFields = HttpLoggingFields.All;
    logging.CombineLogs = true;
});

var app = builder.Build();

app.UseHttpLogging();

app.UseStaticFiles();

app.MapGet("/", () => "Running!");
app.MapGet("/status.json", IResult (HttpContext theContext) =>
{
    var statusResult = new StatusResult(true, 
        $".NET {Environment.Version}", 
        Environment.Version.ToString(),
        DateTime.UtcNow.ToString("o"),
        Environment.GetEnvironmentVariable("LASTMOD") ?? "(local)",
        Environment.GetEnvironmentVariable("COMMIT") ?? "(local)"
    );

    var callback = theContext.Request.Query["callback"];
    if (string.IsNullOrEmpty(callback)) {
        return Results.Json(statusResult);
    }
    string json = JsonSerializer.Serialize(statusResult);
    return Results.Content($"{callback}({json});", "application/javascript");
});

app.MapPost("/test.json", static async (HttpContext theContext) =>
{   
    // read form variables
    var form = await theContext.Request.ReadFormAsync();
    var regex = form["regex"].FirstOrDefault();
    var replacement = form["replacement"].FirstOrDefault();
    var input = form["input"].ToArray() ?? new string[] { };

    var html = $"{regex} {replacement} {input.Length} {input.FirstOrDefault()}";

    var testOutput = new TestOutput(true, html);

    var callback = theContext.Request.Query["callback"];
    if (string.IsNullOrEmpty(callback)) {
        return Results.Json(testOutput);
    }
    string json = JsonSerializer.Serialize(testOutput);
    return Results.Content($"{callback}({json});", "application/javascript");
});

var hostname = Environment.GetEnvironmentVariable("HOSTNAME") ?? "0.0.0.0";
var port = Environment.GetEnvironmentVariable("PORT") ?? "4000";
var url = $"http://{hostname}:{port}";

app.Logger.LogInformation($"App started on {url}", url); 

app.Run(url);

record StatusResult(Boolean success, string tech, string version, string timestamp, string lastmod, string commit);
record TestOutput(Boolean success, string html);


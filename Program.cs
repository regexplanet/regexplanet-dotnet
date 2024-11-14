using System.Text.Json;
using Microsoft.AspNetCore.HttpLogging;

static async Task<bool> handleJsonp(HttpContext theContext, object data)
{
    string jsonStr = JsonSerializer.Serialize(data);

    var response = theContext.Response;
    var callback = theContext.Request.Query["callback"];
    theContext.Response.Clear();
    if (string.IsNullOrEmpty(callback))
    {
        response.ContentType = "application/json";
        response.Headers.Append("Access-Control-Allow-Origin", "*");
        response.Headers.Append("Access-Control-Allow-Methods", "GET");
        response.Headers.Append("Access-Control-Max-Age", "604800");
        await response.WriteAsync(jsonStr);
    } else {
        response.ContentType = "text/javascript; charset=utf-8";
        await response.WriteAsync($"{callback}({jsonStr});");
    }
    return true;
}

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHttpLogging(logging =>
{
    logging.LoggingFields = HttpLoggingFields.All;
    logging.CombineLogs = true;
});

var app = builder.Build();

app.MapFallback(async theContext =>
{
    theContext.Response.StatusCode = 404;

    var result = new TestOutput(false, "404 Not Found", "");

    await handleJsonp(theContext, result);
});

app.UseHttpLogging();

app.UseStaticFiles();

app.MapGet("/", () => "Running!");
app.MapGet("/status.json", static async (HttpContext theContext) =>
{
    var statusResult = new StatusResult(true, 
        $".NET {Environment.Version}", 
        Environment.Version.ToString(),
        DateTime.UtcNow.ToString("o"),
        Environment.GetEnvironmentVariable("LASTMOD") ?? "(local)",
        Environment.GetEnvironmentVariable("COMMIT") ?? "(local)"
    );

    await handleJsonp(theContext, statusResult);
});

app.MapPost("/test.json", RunTest);
app.MapGet("/test.json", RunTest);

static async Task RunTest(HttpContext theContext)
{   
    // read form variables
    var form = await theContext.Request.ReadFormAsync();
    var regex = form["regex"].FirstOrDefault();
    var replacement = form["replacement"].FirstOrDefault();
    var input = form["input"].ToArray() ?? new string[] { };

    var html = $"{regex} {replacement} {input.Length} {input.FirstOrDefault()}";

    var testOutput = new TestOutput(true, "", html);

    await handleJsonp(theContext, testOutput);
}

var hostname = Environment.GetEnvironmentVariable("HOSTNAME") ?? "0.0.0.0";
var port = Environment.GetEnvironmentVariable("PORT") ?? "4000";
var url = $"http://{hostname}:{port}";

app.Logger.LogInformation($"App started on {url}", url); 

app.Run(url);

record StatusResult(Boolean success, string tech, string version, string timestamp, string lastmod, string commit);
record TestOutput(Boolean success, string message, string html);


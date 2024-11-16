using System.Text.Json;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.Extensions.Primitives;

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

    await JsonpHandler.handleJsonp(theContext, result);
});

app.UseHttpLogging();

app.UseStaticFiles();

app.MapGet("/", () => $".NET {Environment.Version}");
app.MapGet("/status.json", static async (HttpContext theContext) =>
{
    var statusResult = new StatusResult(true, 
        $".NET {Environment.Version}", 
        Environment.Version.ToString(),
        DateTime.UtcNow.ToString("o"),
        Environment.GetEnvironmentVariable("LASTMOD") ?? "(local)",
        Environment.GetEnvironmentVariable("COMMIT") ?? "(local)"
    );

    await JsonpHandler.handleJsonp(theContext, statusResult);
});

app.MapPost("/test.json", PostRunTest);
app.MapGet("/test.json", GetRunTest);

static string[] getStrings(StringValues rawValues) {

    return rawValues.Where(i => i != null).Select(i => i!).ToArray() ?? new string[] { };
}

static async Task GetRunTest(HttpContext theContext)
{
    var regex = theContext.Request.Query["regex"].FirstOrDefault() ?? "";
    var replacement = theContext.Request.Query["replacement"].FirstOrDefault() ?? "";
    var options = getStrings(theContext.Request.Query["option"]);
    var inputs = getStrings(theContext.Request.Query["input"]);

    var testInput = new TestInput(regex, replacement, options, inputs);
    var testOutput = TestRunner.RunTest(testInput);

    await JsonpHandler.handleJsonp(theContext, testOutput);
}


static async Task PostRunTest(HttpContext theContext)
{
    var form = await theContext.Request.ReadFormAsync();
    var regex = form["regex"].FirstOrDefault() ?? "";
    var replacement = form["replacement"].FirstOrDefault() ?? "";
    var options = getStrings(form["option"]);
    var inputs = getStrings(form["input"]);

    var testInput = new TestInput(regex, replacement, options, inputs);
    var testOutput = TestRunner.RunTest(testInput);

    await JsonpHandler.handleJsonp(theContext, testOutput);
}

var hostname = Environment.GetEnvironmentVariable("HOSTNAME") ?? "0.0.0.0";
var port = Environment.GetEnvironmentVariable("PORT") ?? "4000";
var url = $"http://{hostname}:{port}";

app.Logger.LogInformation($"App started on {url}", url); 

app.Run(url);

record StatusResult(Boolean success, string tech, string version, string timestamp, string lastmod, string commit);


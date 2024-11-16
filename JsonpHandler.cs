using Microsoft.AspNetCore.Http;
using System.Text.Json;

public static class JsonpHandler
{
    public static async Task<bool> handleJsonp(HttpContext theContext, object data)
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
        }
        else
        {
            response.ContentType = "text/javascript; charset=utf-8";
            await response.WriteAsync($"{callback}({jsonStr});");
        }
        return true;
    }
}
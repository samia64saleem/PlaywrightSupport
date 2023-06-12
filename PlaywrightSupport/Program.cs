using Microsoft.Playwright;
using System.Text.Json;

class Program
{
    static async Task Main()
    {
        var playwright = await Playwright.CreateAsync();

        var requestContext = await playwright.APIRequest.NewContextAsync(new APIRequestNewContextOptions()
        {
            BaseURL = "https://example.com"
        });

        var createFormData = requestContext.CreateFormData();
        createFormData.Set("username", "aa");
        createFormData.Set("password", "bb");

        IAPIResponse responseFetchAsync = await requestContext.FetchAsync("/api/login", new()
        {
            Method = "post",
            Multipart = createFormData
        });

        getCookie(responseFetchAsync);

        Console.WriteLine("Before: " + await requestContext.StorageStateAsync());
        Console.WriteLine("After: " + await requestContext.StorageStateAsync());
    }

    public static async Task<string> getCookie(IAPIResponse responseFetchAsync)
    {
        JsonElement? responseContent = await responseFetchAsync.JsonAsync();
        var formattedJson = JsonSerializer.Serialize(responseContent, new JsonSerializerOptions
        {
            WriteIndented = true
        });
        Console.WriteLine("responseFetchAsync: " + formattedJson);

        if (responseFetchAsync.Headers.TryGetValue("set-cookie", out string cookieV))
        {
            Console.WriteLine("cookie: " + cookieV.Split(";").First());
        }
        return formattedJson;
    }

}

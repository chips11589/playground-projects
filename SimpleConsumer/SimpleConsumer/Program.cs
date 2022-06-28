//using Asos.Identity.Core.Api.TokenGenerator.Adal;
//using Asos.Identity.Core.Api.TokenGenerator.Certificates;
using Microsoft.Identity.Client;
using System.Net.Http.Headers;
//using System.Security.Cryptography.X509Certificates;

Console.WriteLine($"Getting token");

//var certificate = new CertificateFinder().FindByThumbprint(StoreName.My, StoreLocation.CurrentUser, "0B773DF949055288C701B95EC083C7D2AFC874EF")
//                .First();

//var generator = new ClientAssertionCertificateTokenGenerator()
//{
//    ClientId = "a9e89fbc-3875-4c0c-add0-058a230f6dd9",
//    Certificate = certificate,
//    Authority = "https://login.microsoftonline.com/73695d2a-8a88-4cb4-9e96-332954866ca0/oauth2/token",
//    Resource = "api://a9e89fbc-3875-4c0c-add0-058a230f6dd9"
//};

//var token = await generator.GetJwtTokenAsync();

var clientID = Environment.GetEnvironmentVariable("AZURE_CLIENT_ID");
var tokenPath = Environment.GetEnvironmentVariable("AZURE_FEDERATED_TOKEN_FILE");
var tenantID = Environment.GetEnvironmentVariable("AZURE_TENANT_ID");

var confidentialClientApp = ConfidentialClientApplicationBuilder.Create(clientID)
        .WithClientAssertion(ReadJWTFromFS(tokenPath))
        .WithTenantId(tenantID).Build();
var token = await confidentialClientApp.AcquireTokenForClient(new List<string> { "api://a9e89fbc-3875-4c0c-add0-058a230f6dd9/.default" }).ExecuteAsync();

var handler = new HttpClientHandler { ServerCertificateCustomValidationCallback = (request, cert, chain, errors) => true };

using (var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://20.82.227.28/") })
{
    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.AccessToken);
    httpClient.Timeout = TimeSpan.FromHours(1);

    var response = await httpClient.GetAsync("WeatherForecast");
    Console.WriteLine($"Response status: {response.StatusCode}");
}

string ReadJWTFromFS(string tokenPath)
{
    string text = System.IO.File.ReadAllText(tokenPath);
    return text;
}
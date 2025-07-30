using CloudWorks.Api;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace CloudWorks.IntegrationTests.Api.Controllers;

public abstract class TestBaseFactory : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient Client;

    protected TestBaseFactory(WebApplicationFactory<Program> factory)
    {
        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
             
            });
        });
        
        Client = _factory.CreateClient();
    }

    protected async Task<HttpResponseMessage> GetAsync(string url)
    {
        return await Client.GetAsync(url);
    }

    protected async Task<HttpResponseMessage> PostAsync(string url, HttpContent content)
    {
        return await Client.PostAsync(url, content);
    }

    protected async Task<HttpResponseMessage> PutAsync(string url, HttpContent content)
    {
        return await Client.PutAsync(url, content);
    }

    protected async Task<HttpResponseMessage> DeleteAsync(string url)
    {
        return await Client.DeleteAsync(url);
    }

    protected StringContent CreateJsonContent<T>(T obj)
    {
        var json = System.Text.Json.JsonSerializer.Serialize(obj);
        return new StringContent(json, System.Text.Encoding.UTF8, "application/json");
    }
} 
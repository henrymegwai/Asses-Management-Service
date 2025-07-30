using CloudWorks.Api;
using Microsoft.AspNetCore.Mvc.Testing;

namespace CloudWorks.IntegrationTests.Api.Controllers;

[CollectionDefinition("Controller Tests")]
public class ControllerTestCollection : ICollectionFixture<WebApplicationFactory<Program>>
{
    // This class serves as a collection fixture for all controller tests
    // It ensures that the WebApplicationFactory is shared across all test classes
    // and properly disposed of after all tests are complete
}
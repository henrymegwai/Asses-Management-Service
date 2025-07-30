# CloudWorks Integration Tests

This project contains integration tests for the CloudWorks API controllers using xUnit, NSubstitute, and FluentAssertions.

## Project Structure

```
CloudWorks.IntegrationTests/
├── Api.Controllers/
│   ├── TestBase.cs                    # Base test fixture for all controller tests
│   ├── ControllerTestCollection.cs    # Test collection definition
│   ├── AccessPointsControllerTests.cs # Integration tests for AccessPointsController
│   ├── BookingsControllerTests.cs     # Integration tests for BookingsController
│   └── SitesControllerTests.cs        # Integration tests for SitesController
└── README.md                          # This file
```

## Test Coverage

### AccessPointsController Tests
- **GET** `/accesspoints/{siteId}` - Get access points by site ID
- **GET** `/accesspoints/histories/{siteId}` - Get access point histories
- **GET** `/accesspoints/{id}/free-time-slots` - Get free time slots
- **POST** `/accesspoints/create` - Create access point
- **PUT** `/accesspoints/{id}/update` - Update access point
- **POST** `/accesspoints/{id}/open` - Open access point
- **DELETE** `/accesspoints/{id}` - Delete access point

### BookingsController Tests
- **GET** `/bookings` - Get all bookings with pagination and filtering
- **POST** `/bookings/{siteId}/create` - Create booking
- **PUT** `/bookings/{id}/update` - Update booking
- **DELETE** `/bookings/{id}` - Delete booking

### SitesController Tests
- **GET** `/sites` - Get all sites with pagination and filtering
- **GET** `/sites/{id}/users` - Get users in site
- **POST** `/sites/create` - Create site
- **PUT** `/sites/{id}/update` - Update site
- **DELETE** `/sites/{id}/delete` - Delete site

## Test Scenarios

Each controller test covers:
- **Success scenarios** - Valid requests that return expected responses
- **Validation failures** - Invalid requests that return BadRequest
- **Error scenarios** - Database errors, not found scenarios, etc.
- **Edge cases** - Boundary conditions and special cases

## Running the Tests

### Prerequisites
- .NET 8.0 SDK
- All required NuGet packages (automatically restored)

### Running Tests
```bash
# Run all integration tests
dotnet test

# Run specific test class
dotnet test --filter "AccessPointsControllerTests"

# Run with verbose output
dotnet test --verbosity normal

# Run with coverage
dotnet test --collect:"XPlat Code Coverage"
```

## Test Architecture

### TestBase Class
- Provides common HTTP client setup
- Handles WebApplicationFactory configuration
- Includes helper methods for HTTP requests
- Manages JSON serialization for request bodies

### Mocking Strategy
- Uses NSubstitute to mock the IMediator interface
- Each test configures expected responses for specific scenarios
- Tests verify both HTTP status codes and response content

### Assertions
- Uses FluentAssertions for readable assertions
- Verifies HTTP status codes (200 OK, 400 BadRequest, etc.)
- Checks response structure and content where applicable

## Best Practices

1. **Test Isolation**: Each test is independent and doesn't rely on other tests
2. **Descriptive Names**: Test method names clearly describe the scenario being tested
3. **Arrange-Act-Assert**: Tests follow the AAA pattern for clarity
4. **Mocking**: External dependencies are mocked to focus on controller behavior
5. **Error Scenarios**: Tests cover both success and failure paths
6. **Edge Cases**: Tests include boundary conditions and special cases

## Adding New Tests

To add tests for a new controller:

1. Create a new test class inheriting from `TestBase`
2. Add the `[Collection("Controller Tests")]` attribute
3. Mock the IMediator for expected scenarios
4. Write tests covering success, validation, and error scenarios
5. Use descriptive test method names following the pattern: `MethodName_WhenCondition_ShouldReturnExpectedResult`

## Dependencies

- **xUnit**: Testing framework
- **NSubstitute**: Mocking framework
- **FluentAssertions**: Assertion library
- **Microsoft.AspNetCore.Mvc.Testing**: ASP.NET Core testing utilities
- **Microsoft.AspNetCore.TestHost**: Test host for integration testing 
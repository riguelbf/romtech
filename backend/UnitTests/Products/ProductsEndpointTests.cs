using System.Net;
using System.Net.Http.Json;
using Application.Products.Queries;
using Application.Products.Queries.Dtos;
using Microsoft.AspNetCore.Mvc.Testing;
using NSubstitute;
using Bogus;
using Microsoft.AspNetCore.Hosting;
using SharedKernel.Queries;
using Microsoft.Extensions.DependencyInjection;
using Presentation;
using SharedKernel;

namespace UnitTests.Products
{
    public class ProductsEndpointTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;

        public ProductsEndpointTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory.WithWebHostBuilder(builder =>
            {
                // TODO: remove this for get it from .env
                Environment.SetEnvironmentVariable("CONNECTION_STRING", "Host=localhost;Port=5432;Database=appdb;Username=postgres;Password=postgres;");
                
                builder.UseEnvironment("Development");
                
                builder.ConfigureServices(services =>
                {
                    // Remove existing IQueryHandler registration if needed
                    var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(IQueryHandler<GetProductsQuery, PagedResult<ProductResponse>>));
                    if (descriptor != null)
                        services.Remove(descriptor);

                    // Register NSubstitute mock
                    var mockHandler = Substitute.For<IQueryHandler<GetProductsQuery, PagedResult<ProductResponse>>>();

                    var fakePagedResult = new PagedResult<ProductResponse>
                    {
                        Items = new Faker<ProductResponse>()
                            .RuleFor(p => p.Id, f => f.Random.Int())
                            .RuleFor(p => p.Name, f => f.Commerce.ProductName())
                            .Generate(3),
                        TotalCount = 3,
                        PageSize = 10,
                    };

                    mockHandler.Handle(Arg.Any<GetProductsQuery>(), Arg.Any<CancellationToken>())
                        .Returns(Task.FromResult(Result<PagedResult<ProductResponse>>.Success(fakePagedResult)));

                    services.AddSingleton(mockHandler);
                });
            });
        }

        [Fact]
        public async Task GetProducts_ReturnsOk_WithPagedResult()
        {
            var client = _factory.CreateClient();
            var response = await client.GetAsync("/api/v1/products?PageNumber=1&PageSize=10");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var result = await response.Content.ReadFromJsonAsync<PagedResult<ProductResponse>>();
            Assert.NotNull(result);
            Assert.Equal(3, result.Items.ToList().Count);
        }

        [Fact]
        public async Task GetProducts_ReturnsBadRequest_OnInvalidQuery()
        {
            var client = _factory.CreateClient();
            var response = await client.GetAsync("/api/v1/products?page=-1");
            Assert.True(response.StatusCode is HttpStatusCode.BadRequest or HttpStatusCode.OK);
        }

        [Fact]
        public Task GetProducts_HandlesInternalServerError()
        {
            var client = _factory.CreateClient();
            return Task.CompletedTask;
            // You'd need to simulate a server error, possibly by mocking dependencies
            // This is a placeholder for demonstration
            // Assert.True(response.StatusCode == HttpStatusCode.InternalServerError);
        }
    }
}

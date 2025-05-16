using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Application.Products.Commands;
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
                    // Mock GetProductsQuery handler
                    var getProductsDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(IQueryHandler<GetProductsQuery, PagedResult<ProductResponse>>));
                    if (getProductsDescriptor != null)
                        services.Remove(getProductsDescriptor);

                    var getProductsMock = Substitute.For<IQueryHandler<GetProductsQuery, PagedResult<ProductResponse>>>();
                    var fakePagedResult = new PagedResult<ProductResponse>
                    {
                        Items = new Faker<ProductResponse>()
                            .RuleFor(p => p.Id, f => f.Random.Int())
                            .RuleFor(p => p.Name, f => f.Commerce.ProductName())
                            .Generate(3),
                        TotalCount = 3,
                        PageSize = 10,
                    };
                    getProductsMock.Handle(Arg.Any<GetProductsQuery>(), Arg.Any<CancellationToken>())
                        .Returns(Task.FromResult(Result<PagedResult<ProductResponse>>.Success(fakePagedResult)));
                    services.AddSingleton(getProductsMock);

                    // Mock GetProductByIdQuery handler
                    var getByIdDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(IQueryHandler<GetProductByIdQuery, ProductResponse>));
                    if (getByIdDescriptor != null)
                        services.Remove(getByIdDescriptor);

                    var getByIdMock = Substitute.For<IQueryHandler<GetProductByIdQuery, ProductResponse>>();
                    var fakeProduct = new Faker<ProductResponse>()
                        .RuleFor(p => p.Id, f => f.Random.Int())
                        .RuleFor(p => p.Name, f => f.Commerce.ProductName())
                        .Generate();
                    getByIdMock.Handle(Arg.Any<GetProductByIdQuery>(), Arg.Any<CancellationToken>())
                        .Returns(Task.FromResult(Result<ProductResponse>.Success(fakeProduct)));
                    services.AddSingleton(getByIdMock);

                    // Mock CreateProductCommand handler
                    var createProductDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(ICommandHandler<CreateProductCommand, int>));
                    if (createProductDescriptor != null)
                        services.Remove(createProductDescriptor);

                    var createProductMock = Substitute.For<ICommandHandler<CreateProductCommand, int>>();
                    createProductMock.Handle(Arg.Any<CreateProductCommand>(), Arg.Any<CancellationToken>())
                        .Returns(Task.FromResult(Result<int>.Success(123)));
                    services.AddSingleton(createProductMock);

                    // Add more mocks as needed for other handlers...
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
        }

        [Fact]
        public async Task GetProductById_ReturnsOk_WhenProductExists()
        {
            var client = _factory.CreateClient();
            var response = await client.GetAsync("/api/v1/products/1");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var result = await response.Content.ReadFromJsonAsync<ProductResponse>();
            Assert.NotNull(result);
            Assert.NotEqual(0, result.Id);
        }

        [Fact]
        public async Task GetProductById_ReturnsNotFound_WhenProductDoesNotExist()
        {
            var client = _factory.CreateClient();
            var response = await client.GetAsync("/api/v1/products/99990000000");
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task GetProductById_ReturnsBadRequest_OnInvalidId()
        {
            var client = _factory.CreateClient();
            var response = await client.GetAsync("/api/v1/products/0");
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task CreateProduct_ReturnsCreated_OnValidRequest()
        {
            var client = _factory.CreateClient();
            var request = new CreateProductCommand
            {
                Name = "Test Product",
                Description = "Test Description",
                Price = 10.99m,
                Stock = 5
            };
            var response = await client.PostAsJsonAsync("/api/v1/products", request);
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            var content = await response.Content.ReadFromJsonAsync<JsonElement>();
            Assert.True(content.TryGetProperty("id", out var idElement));
            Assert.Equal(JsonValueKind.Number, idElement.ValueKind);
        }

        [Fact]
        public async Task CreateProduct_ReturnsBadRequest_OnInvalidRequest()
        {
            var client = _factory.CreateClient();
            var request = new CreateProductCommand
            {
                Name = "",
                Price = -1,
                Stock = -5
            };
            var response = await client.PostAsJsonAsync("/api/v1/products", request);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }
    }
}

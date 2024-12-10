using System.Net;
using Moq;
using Moq.Protected;
using Newtonsoft.Json.Linq;
using OrderApi.Client;

namespace OrderApi.UnitTests;

[TestFixture]
public class ApiClientTests
{
    private Mock<HttpMessageHandler> _mockHttpMessageHandler;
    private HttpClient _mockHttpClient;
    private ApiClient _apiClient;

    [SetUp]
    public void SetUp()
    {
        // Setup mock for HttpClient
        _mockHttpMessageHandler = new Mock<HttpMessageHandler>(MockBehavior.Default);
        _mockHttpClient = new HttpClient(_mockHttpMessageHandler.Object);
        _apiClient = new ApiClient(_mockHttpClient);
    }

    [TearDown]
    public void TearDown()
    {
        _mockHttpClient.Dispose();
        _mockHttpMessageHandler.Reset();
    }

    private void SetupHttpResponse(HttpStatusCode statusCode, string content)
    {
        _mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = statusCode,
                Content = new StringContent(content),
            })
            .Verifiable();
    }

    [Test]
    public async Task FetchMedicalEquipmentOrders_ShouldReturnOrdersOnSuccess()
    {
        // Arrange
        var jsonResponse = @"[{ 'OrderId': '123' }, { 'OrderId': '456' }]";
        SetupHttpResponse(HttpStatusCode.OK, jsonResponse);

        // Act
        var result = await _apiClient.FetchMedicalEquipmentOrders();

        // Assert
        Assert.IsNotEmpty(result);
        Assert.AreEqual(2, result.Length);
    }

    [Test]
    public async Task FetchMedicalEquipmentOrders_ShouldLogErrorOnFailure()
    {
        // Arrange
        SetupHttpResponse(HttpStatusCode.InternalServerError, "Error");

        // Act
        var result = await _apiClient.FetchMedicalEquipmentOrders();

        // Assert
        Assert.IsEmpty(result);
    }

    [Test]
    public void IncrementDeliveryNotification_ShouldIncrementValue()
    {
        // Arrange
        var item = JObject.Parse(@"{ ""deliveryNotification"": 1 }");

        // Act
        _apiClient.IncrementDeliveryNotification(item);

        // Assert
        Assert.That(2, Is.EqualTo((int)item["deliveryNotification"]));
    }
}
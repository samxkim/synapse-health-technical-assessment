using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using OrderApi.Controllers;
using OrderApi.Service;

namespace OrderApi.UnitTests;

[TestFixture]
public class OrderUpdateControllerTests
{
    private Mock<IOrderService> _mockOrderService;
    private OrderUpdateController _orderUpdateController;

    [SetUp]
    public void SetUp()
    {
        _mockOrderService = new Mock<IOrderService>();
        _orderUpdateController = new OrderUpdateController(_mockOrderService.Object);
    }

    [Test]
    public async Task GetMedicalEquipmentOrders_ShouldReturnOkResult()
    {
        _mockOrderService.Setup(s => s.HandleOrders()).Verifiable();

        var result = await _orderUpdateController.GetMedicalEquipmentOrders();

        result.Should().BeOfType<OkResult>();
        _mockOrderService.Verify(s => s.HandleOrders(), Times.Once);
    }
}
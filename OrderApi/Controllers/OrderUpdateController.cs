using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using OrderApi.Service;
using Serilog;

namespace OrderApi.Controllers
{
    /// <summary>
    /// I Get a list of orders from the API
    /// I check if the order is in a delivered state, If yes then send a delivery alert and add one to deliveryNotification
    /// I then update the order.
    ///
    /// Personal Notes:
    /// This is under the assumption that this API isn't handling the lifecycle of an ongoing background task
    /// Only that if the API endpoint is called, then
    /// </summary>
    /// [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[action]")]
    public class OrderUpdateController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderUpdateController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpGet("medical-equipment")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ActionName("orders")]
        public async Task<IActionResult> GetMedicalEquipmentOrders()
        {
            _orderService.HandleOrders();
            Log.Logger.Information("Results sent to relevant APIs");

            return Ok();
        }
    }
}
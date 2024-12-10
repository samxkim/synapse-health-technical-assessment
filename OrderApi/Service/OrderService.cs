using Newtonsoft.Json.Linq;
using OrderApi.Client;
using OrderApi.Models;
using Serilog;

namespace OrderApi.Service;

public class OrderService : IOrderService
{
    private readonly ApiClient _apiClient;

    public OrderService(ApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public void HandleOrders()
    {
        Log.Logger.Information("Fetching medical equipment orders started");


        var medicalEquipmentOrders = _apiClient.FetchMedicalEquipmentOrders().GetAwaiter().GetResult();
        Log.Logger.Information("Fetched {Count} medical equipment orders", medicalEquipmentOrders.Length);

        foreach (var order in medicalEquipmentOrders)
        {
            Log.Logger.Verbose("Processing order with OrderId: {OrderId}", order["OrderId"]);
            var updatedOrder = ProcessOrder(order);
            _apiClient.SendAlertAndUpdateOrder(updatedOrder).GetAwaiter().GetResult();
            Log.Logger.Verbose("Order with OrderId: {OrderId} has been updated successfully", order["OrderId"]);
        }
    }

    private JObject ProcessOrder(JObject order)
    {
        var items = order["Items"].ToObject<JArray>();
        Log.Logger.Verbose("Processing {Count} items for OrderId: {OrderId}", items.Count, order["OrderId"]);

        foreach (var item in items)
        {
            Log.Logger.Verbose("Checking if item is delivered for OrderId: {OrderId}, Item: {Description}",
                order["OrderId"], item["Description"]);
            if (IsItemDelivered(item))
            {
                Log.Logger.Verbose("Item delivered for OrderId: {OrderId}, Item: {Description}. Sending alert",
                    order["OrderId"], item["Description"]);
                _apiClient.SendAlertMessage(item, order["OrderId"].ToString());

                _apiClient.IncrementDeliveryNotification(item);
                Log.Logger.Verbose("Delivery notification incremented for OrderId: {OrderId}, Item: {Description}",
                    order["OrderId"], item["Description"]);
            }
        }

        return order;
    }

    private static bool IsItemDelivered(JToken item)
    {
        return item["Status"].ToString().Equals("Delivered", StringComparison.OrdinalIgnoreCase);
    }
}
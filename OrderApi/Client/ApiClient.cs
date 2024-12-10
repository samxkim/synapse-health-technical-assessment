using Newtonsoft.Json.Linq;
using OrderApi.Models;
using Serilog;

namespace OrderApi.Client;

public class ApiClient
{
    private readonly HttpClient _httpClient;

    public ApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    /// <summary>
    /// Delivery alert
    /// </summary>
    /// <param name="item">JSON data for the alert</param>
    /// <param name="orderId">The order id for the alert</param>
    public void SendAlertMessage(JToken item, string orderId)
    {
        var alertData = new
        {
            Message = $"Alert for delivered item: Order {orderId}, Item: {item["Description"]}, " +
                      $"Delivery Notifications: {item["deliveryNotification"]}"
        };
        var content = new StringContent(JObject.FromObject(alertData).ToString(), System.Text.Encoding.UTF8,
            "application/json");
        var response = _httpClient.PostAsync(ApiUrls.Alerts, content).Result;

        if (response.IsSuccessStatusCode)
        {
            Log.Logger.Information("Alert sent for delivered item: {}", item["Description"]);
        }
        else
        {
            Log.Logger.Error("Failed to send alert for delivered item: {}", item["Description"]);
        }
    }

    public async Task<JObject[]> FetchMedicalEquipmentOrders()
    {
        string ordersApiUrl = ApiUrls.Orders;
        var response = await _httpClient.GetAsync(ordersApiUrl);
        if (response.IsSuccessStatusCode)
        {
            var ordersData = await response.Content.ReadAsStringAsync();
            return JArray.Parse(ordersData).ToObject<JObject[]>();
        }
        else
        {
            Log.Logger.Error("Failed to fetch orders from API: {}", ordersApiUrl);
            return new JObject[0];
        }
    }

    public void IncrementDeliveryNotification(JToken item)
    {
        item["deliveryNotification"] = item["deliveryNotification"].Value<int>() + 1;
    }

    public async Task SendAlertAndUpdateOrder(JObject order)
    {
        var content = new StringContent(order.ToString(), System.Text.Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync(ApiUrls.Update, content);

        if (response.IsSuccessStatusCode)
        {
            Log.Logger.Information("Updated order for processing: OrderId {}", order["OrderId"]);
        }
        else
        {
            Log.Logger.Error("Failed to send updated order for processing: OrderId {}", order["OrderId"]);
        }
    }
}
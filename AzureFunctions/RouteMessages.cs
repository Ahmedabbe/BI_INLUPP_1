using IoTHubTrigger = Microsoft.Azure.WebJobs.EventHubTriggerAttribute;

using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Azure.EventHubs;
using System.Text;
using System.Net.Http;
using Microsoft.Extensions.Logging;
using AzureFunctions.Models;
using Newtonsoft.Json;
using System;

namespace AzureFunctions
{
    public static class RouteMessages
    {
        private static HttpClient client = new HttpClient();

        [FunctionName("RouteMessages")]
        [return: Table("Measurements")]
        public static TableMeasurementModel Run(
            [IoTHubTrigger("messages/events", Connection = "IotHubConnection", ConsumerGroup = "tablestorage")]EventData message, ILogger log)
        {
            try
            {
                var _data = JsonConvert.DeserializeObject<dynamic>(Encoding.UTF8.GetString(message.Body.Array));
                var _deviceId = message.SystemProperties["iothub-connection-device-id"].ToString();
                var _deviceType = message.Properties["deviceType"].ToString();
                var _latitude = message.Properties["latitude"].ToString();
                var _longitude = message.Properties["longitude"].ToString();
                var _epochTime = message.Properties["epochTime"].ToString();

                var _table = new TableMeasurementModel()
                {
                    PartitionKey = "Measurement",
                    RowKey = Guid.NewGuid().ToString(),
                    deviceId = _deviceId,
                    deviceType = _deviceType,
                    epochTime = _epochTime,
                    latitude = _latitude,
                    longitude = _longitude,
                    temperature = _data.temperature,
                    humidity = _data.humidity
                };
                _table.ConvertEpochTime();

                var _tablejson = JsonConvert.SerializeObject(_table);
                log.LogInformation($"Measurement was saved to Table Storage, Message:: {_tablejson}");
                return _table;
            }
            catch (Exception e)
            {
                log.LogInformation($"Unable to process Request, Error:: {e.Message}");
                return null;
            }

        }
    }
}
using System;
using System.Collections.Generic;
using System.Text;

namespace AzureFunctions.Models
{
    public class MeasurementModel
    {
        public string deviceId { get; set; }
        public string deviceType { get; set; }
        public string epochTime { get; set; }
        public DateTime measurementTime { get; private set; }

        public void ConvertEpochTime()
        {
            DateTimeOffset dateTimeOffset = DateTimeOffset.FromUnixTimeSeconds(long.Parse(epochTime));
            measurementTime = dateTimeOffset.DateTime;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace AzureFunctions.Models
{
    public class TableMeasurementModel : MeasurementModel
    {
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }


        public string latitude { get; set; }
        public string longitude { get; set; }
        public double temperature { get; set; }
        public double humidity { get; set; }
    }
}

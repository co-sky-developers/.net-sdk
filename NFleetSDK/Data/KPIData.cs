using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NFleet.Data
{
    public class KPIData
    {
        public double AccumulatedTravelTimeWithCargo { get; set; }

        public double AccumulatedTravelTimeEmpty { get; set; }

        public double AccumulatedLoadingTime { get; set; }

        public double AccumulatedWaitingTime { get; set; }

        public double AccumulatedWorkingTime { get; set; }

        public double AccumulatedTravelDistance { get; set; }

        public double AccumulatedPickups { get; set; }

        public double AccumulatedDeliveries { get; set; }

        public double LoadPercentage { get; set; }

        public double HighestLoadPercentage { get; set; }

        public double TravelTimeWithCargoPercentage { get; set; }

        public double TravelTimeEmptyPercentage { get; set; }

        public double LoadingTimePercentage { get; set; }

        public double WaitingTimePercentage { get; set; }
    }
}

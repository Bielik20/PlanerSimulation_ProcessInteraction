using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanerSimulation_ProcessInteraction.Statistics
{
    interface IStatistics
    {
        void CollectProcess(double CPUAwaitTime, double IOAwaitTime, double processingTime);
        void CollectProcessor(double durration, int index);
        void CollectClockTime(double clockTime);
    }
}

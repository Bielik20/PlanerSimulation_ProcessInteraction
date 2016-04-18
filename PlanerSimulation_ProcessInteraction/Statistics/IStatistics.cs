using PlanerSimulation_ProcessInteraction.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanerSimulation_ProcessInteraction.Statistics
{
    interface IStatistics
    {
        int TerminatedProcessCount { get; }

        void Initialization(Supervisor mySupervisor);
        void CollectProcess(double CPUAwaitTime, double IOAwaitTime, double processingTime);
        void CollectCPU(double durration, int index);
        void CollectClockTime(double clockTime);
        void Finalization();
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PlanerSimulation_ProcessInteraction.Models;

namespace PlanerSimulation_ProcessInteraction.Statistics
{
    class LStats : IStatistics
    {
        public int TerminatedProcessCount { get; private set; }
        public Results MyResults { get; set; }
        private int StabilityPoint { get; set; }

        public LStats(int StabilityPoint)
        {
            MyResults = new Results(0);
            this.StabilityPoint = StabilityPoint;
        }

        public struct Results
        {
            public double CPUAwaitTime { get; set; }

            public Results(double CPUAwaitTime)
            {
                this.CPUAwaitTime = CPUAwaitTime;
            }

            #region Operators
            public static Results operator +(Results r1, Results r2)
            {
                r1.CPUAwaitTime += r2.CPUAwaitTime;
                return r1;
            }
            public static Results operator +(Results r, double d)
            {
                r.CPUAwaitTime += d;
                return r;
            }
            public static Results operator /(Results r, int dev)
            {
                r.CPUAwaitTime /= dev;
                return r;
            }
            #endregion
        }

        #region IStatistics
        public void CollectClockTime(double clockTime)
        { }

        public void CollectProcess(double CPUAwaitTime, double IOAwaitTime, double processingTime)
        {
            TerminatedProcessCount++;

            if (TerminatedProcessCount > StabilityPoint)
            {
                MyResults += CPUAwaitTime;
            }
        }

        public void CollectCPU(double durration, int index)
        { }

        public void Finalization()
        {
            MyResults /= TerminatedProcessCount - StabilityPoint;
        }

        public void Initialization(Supervisor mySupervisor)
        { }
        #endregion
    }
}

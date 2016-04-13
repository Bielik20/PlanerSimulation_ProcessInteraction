using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PlanerSimulation_ProcessInteraction.Models;

namespace PlanerSimulation_ProcessInteraction.Statistics
{
    class SPStats : IStatistics
    {
        public int TerminatedProcessCount { get; private set; }
        public List<Results> ResultsList { get; set; }

        public SPStats()
        {
            TerminatedProcessCount = 0;
            ResultsList = new List<Results>();
        }

        public struct Results
        {
            public int TerminatedProcessCount { get; set; }
            public double ProcessingTime { get; set; }
            public double CPUAwaitTime { get; set; }

            public Results(int TerminatedProcessCount, double CPUAwaitTime, double ProcessingTime)
            {
                this.TerminatedProcessCount = TerminatedProcessCount;
                this.CPUAwaitTime = CPUAwaitTime;
                this.ProcessingTime = ProcessingTime;
            }

            #region Operators
            public static Results operator +(Results r1, Results r2)
            {
                r1.ProcessingTime += r2.ProcessingTime;
                r1.CPUAwaitTime += r2.CPUAwaitTime;
                return r1;
            }
            public static Results operator /(Results r, int dev)
            {
                r.ProcessingTime /= dev;
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
            ResultsList.Add(new Results(TerminatedProcessCount, CPUAwaitTime, processingTime));
        }

        public void CollectProcessor(double durration, int index)
        { }

        public void Finalization()
        { }

        public void Initialization(Supervisor mySupervisor)
        { }
        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PlanerSimulation_ProcessInteraction.Statistics
{
    class SimResults : IStatistics
    {
        #region Helper Properties
        public double ClockTime { get; set; }
        private int TerminatedProcessCount { get; set; }
        private double CPUAllAwaitTime { get; set; }
        private double IOAllAwaitTime { get; set; }
        private double ProcessingAllTime { get; set; }
        private double[] CPUOccupationTime { get; set; }
        #endregion


        public List<Results> ResultsList { get; private set; } = new List<Results>();

        public struct Results
        {
            public int terminatedProcessCount { get; set; }
            public double terminatedProcessesInTime { get; set; }
            public double avrProcessingTime { get; set; }
            public double avrCPUAwaitTime { get; set; }
            public double avrIOAwaitTime { get; set; }
            public double[] avrCPUOccupation { get; set; }

            public Results(int numberOfProcessors)
            {
                terminatedProcessCount = 0;
                terminatedProcessesInTime = 0;
                avrProcessingTime = 0;
                avrCPUAwaitTime = 0;
                avrIOAwaitTime = 0;
                avrCPUOccupation = new double[numberOfProcessors];
                for (int i = 0; i < numberOfProcessors; i++)
                {
                    avrCPUOccupation[i] = 0;
                }
            }
        }

        public SimResults(int numberOfProcessors)
        {
            CPUOccupationTime = new double[numberOfProcessors];
            for (int i = 0; i < numberOfProcessors; i++)
            {
                CPUOccupationTime[i] = 0;
            }

            ClockTime = 0;
            TerminatedProcessCount = 0;
            CPUAllAwaitTime = 0;
            IOAllAwaitTime = 0;
            ProcessingAllTime = 0;
        }

        private void UpdateResults()
        {
            if (TerminatedProcessCount == 0)
                return;

            var _newResult = new Results(CPUOccupationTime.Count());
            _newResult.terminatedProcessCount = TerminatedProcessCount;
            _newResult.terminatedProcessesInTime = TerminatedProcessCount / ClockTime;
            _newResult.avrProcessingTime = ProcessingAllTime / TerminatedProcessCount;
            _newResult.avrCPUAwaitTime = CPUAllAwaitTime / TerminatedProcessCount;
            _newResult.avrIOAwaitTime = IOAllAwaitTime / TerminatedProcessCount;
            for (int i = 0; i < CPUOccupationTime.Count(); i++)
            {
                _newResult.avrCPUOccupation[i] = CPUOccupationTime[i] / ClockTime;
            }

            ResultsList.Add(_newResult);
        }


        #region IStatistics
        public void CollectClockTime(double clockTime)
        {
            this.ClockTime = clockTime;
        }

        public void CollectProcess(double CPUAwaitTime, double IOAwaitTime, double processingTime)
        {
            this.TerminatedProcessCount++;

            if (double.IsNaN(CPUAwaitTime))
                this.CPUAllAwaitTime += 0;
            else
                this.CPUAllAwaitTime += CPUAwaitTime;

            if (double.IsNaN(IOAwaitTime))
                this.IOAllAwaitTime += 0;
            else
                this.IOAllAwaitTime += IOAwaitTime;

            if (double.IsNaN(processingTime))
                this.ProcessingAllTime += 0;
            else
                this.ProcessingAllTime += processingTime;

            UpdateResults();
        }

        public void CollectProcessor(double durration, int index)
        {
            if (double.IsNaN(durration))
                CPUOccupationTime[index] += 0;
            else
                CPUOccupationTime[index] += durration;
        }
        #endregion
    }
}

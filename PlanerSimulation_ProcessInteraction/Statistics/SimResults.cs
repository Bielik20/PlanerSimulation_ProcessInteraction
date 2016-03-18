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
        private double ClockTime { get; set; }
        private int TerminatedProcessCount
        {
            get { return _terminatedProcessCount; }
            set { _terminatedProcessCount = value; UpdateResults(); }
        }
        private int _terminatedProcessCount;
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
            public double avrCPUAwaitTime { get; set; }
            public double avrIOAwaitTime { get; set; }
            public double avrProcessingTime { get; set; }
            public double[] avrCPUOccupation { get; set; }

            public Results(int numberOfProcessors)
            {
                terminatedProcessCount = 0;
                terminatedProcessesInTime = 0;
                avrCPUAwaitTime = 0;
                avrIOAwaitTime = 0;
                avrProcessingTime = 0;
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
            var _newResult = new Results(CPUOccupationTime.Count());
            _newResult.terminatedProcessCount = TerminatedProcessCount;
            _newResult.terminatedProcessesInTime = TerminatedProcessCount / ClockTime;
            _newResult.avrCPUAwaitTime = CPUAllAwaitTime / TerminatedProcessCount;
            _newResult.avrIOAwaitTime = IOAllAwaitTime / TerminatedProcessCount;
            _newResult.avrProcessingTime = ProcessingAllTime / ClockTime;
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
            this.CPUAllAwaitTime += CPUAllAwaitTime;
            this.IOAllAwaitTime += IOAllAwaitTime;
            this.ProcessingAllTime += ProcessingAllTime;
            this.TerminatedProcessCount++;
        }

        public void CollectProcessor(double durration, int index)
        {
            CPUOccupationTime[index] += durration;
        }
        #endregion
    }
}

using PlanerSimulation_ProcessInteraction.Models;
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
        public double ClockTime { get; private set; }
        private double StabilityTime { get; set; }
        private double RelativeTime{ get { return ClockTime - StabilityTime; } }
        public int TerminatedProcessCount { get; private set; }
        private int StabilityPoint { get; set; }
        private int RelativeTermintedProcessCount { get { return TerminatedProcessCount - StabilityPoint; } }
        private double CPUAllAwaitTime { get; set; }
        private double IOAllAwaitTime { get; set; }
        private double ProcessingAllTime { get; set; }
        private double[] CPUOccupationTime { get; set; }
        private bool Flag { get; set; }
        #endregion

        //------------------------------------------------------------------

        public List<Results> ResultsList { get; private set; } = new List<Results>();

        public struct Results
        {
            public int terminatedProcessCount { get; set; }
            public double terminatedProcessesInTime { get; set; }
            public double avrProcessingTime { get; set; }
            public double avrCPUAwaitTime { get; set; }
            public double avrIOAwaitTime { get; set; }
            public double[] avrCPUOccupation { get; set; }

            public Results(int numberOfCPUs)
            {
                terminatedProcessCount = 0;
                terminatedProcessesInTime = 0;
                avrProcessingTime = 0;
                avrCPUAwaitTime = 0;
                avrIOAwaitTime = 0;
                avrCPUOccupation = new double[numberOfCPUs];
                for (int i = 0; i < numberOfCPUs; i++)
                {
                    avrCPUOccupation[i] = 0;
                }
            }
        }

        public SimResults(int StabilityPoint)
        {
            this.StabilityPoint = StabilityPoint;
            StabilityTime = 0;
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
            if (RelativeTime == 0)
                _newResult.terminatedProcessesInTime = 0;
            else
                _newResult.terminatedProcessesInTime = RelativeTermintedProcessCount / RelativeTime;
            _newResult.avrProcessingTime = ProcessingAllTime / RelativeTermintedProcessCount;
            _newResult.avrCPUAwaitTime = CPUAllAwaitTime / RelativeTermintedProcessCount;
            _newResult.avrIOAwaitTime = IOAllAwaitTime / RelativeTermintedProcessCount;
            for (int i = 0; i < CPUOccupationTime.Count(); i++)
            {
                if (RelativeTime == 0)
                    _newResult.avrCPUOccupation[i] = 0;
                else
                    _newResult.avrCPUOccupation[i] = CPUOccupationTime[i] / RelativeTime;
            }

            ResultsList.Add(_newResult);
        }

        //------------------------------------------------------------------

        #region IStatistics
        public void Initialization(Supervisor mySupervisor)
        {
            var numberOfCPUs = mySupervisor.myCPUs.Count();
            CPUOccupationTime = new double[numberOfCPUs];
            for (int i = 0; i < numberOfCPUs; i++)
            {
                CPUOccupationTime[i] = 0;
            }
        }

        public void CollectClockTime(double clockTime)
        {
            this.ClockTime = clockTime;
        }

        public void CollectProcess(double CPUAwaitTime, double IOAwaitTime, double processingTime)
        {
            this.TerminatedProcessCount++;

            if (TerminatedProcessCount == StabilityPoint - 2)
                StabilityTime = ClockTime;
            if (TerminatedProcessCount > StabilityPoint && !Flag)
            {
                Flag = true;
            }
            if (!Flag)
                return;

            this.CPUAllAwaitTime += CPUAwaitTime;
            if (double.IsNaN(IOAwaitTime))
                this.IOAllAwaitTime += 0;
            else
                this.IOAllAwaitTime += IOAwaitTime;
            this.ProcessingAllTime += processingTime;

            UpdateResults();
        }

        public void CollectProcessor(double durration, int index)
        {
            if (!Flag)
                return;

            CPUOccupationTime[index] += durration;
        }

        public void Finalization()
        {
        }
        #endregion
    }
}

using PlanerSimulation_ProcessInteraction.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanerSimulation_ProcessInteraction.Statistics
{
    class ResultTracker : IStatistics
    {
        #region Helper Properties
        public double ClockTime { get; private set; }
        private double LastTime { get; set; }
        private double TimeSpan
        {
            get
            {
                var _temp = ClockTime - LastTime;
                if (_temp == 0)
                    return double.PositiveInfinity;
                else
                    return _temp;
            }
        }
        public int TerminatedProcessCount { get; private set; }
        private int StabilityPoint { get; set; }
        //----------------------------------------------
        private double[] CPUOccupationTime { get; set; }
        private double CPUAwaitTime { get; set; }
        private double IOAwaitTime { get; set; }
        private double ProcessingTime { get; set; }
        //----------------------------------------------
        public double TermProcInTime { get; set; }
        public double AvrProcessingTime { get; set; }
        public double AvrCPUAwaitTime { get; set; }
        public double AvrIOAwaitTime { get; set; }
        public double AvrCPUOccupation { get; set; }
        #endregion

        //------------------------------------------------------------------

        public List<Results> ResultsList { get; private set; } = new List<Results>();

        public struct Results
        {
            public int TerminatedProcessCount { get; set; }
            public double TerminatedProcessesInTime { get; set; }
            public double ProcessingTime { get; set; }
            public double CPUAwaitTime { get; set; }
            public double IOAwaitTime { get; set; }
            public double[] CPUOccupation { get; set; }

            public Results(int numberOfCPUs)
            {
                TerminatedProcessCount = 0;
                TerminatedProcessesInTime = 0;
                ProcessingTime = 0;
                CPUAwaitTime = 0;
                IOAwaitTime = 0;
                CPUOccupation = new double[numberOfCPUs];
                for (int i = 0; i < numberOfCPUs; i++)
                {
                    CPUOccupation[i] = 0;
                }
            }
        }

        public ResultTracker(int StabilityPoint)
        { 
            this.StabilityPoint = StabilityPoint;
            ClockTime = 0;
            LastTime = 0;
            TerminatedProcessCount = 0;

        }

        private void UpdateResults()
        {
            var _newResult = new Results(CPUOccupationTime.Count());
            _newResult.TerminatedProcessCount = TerminatedProcessCount;
            _newResult.TerminatedProcessesInTime = 1 / TimeSpan;
            _newResult.ProcessingTime = ProcessingTime;
            _newResult.CPUAwaitTime = CPUAwaitTime;
            _newResult.IOAwaitTime = IOAwaitTime;
            for (int i = 0; i < CPUOccupationTime.Count(); i++)
            {
                _newResult.CPUOccupation[i] = CPUOccupationTime[i] / TimeSpan;
            }

            ResultsList.Add(_newResult);

            //Cleaning data before next check
            LastTime = ClockTime;
            for (int i = 0; i < CPUOccupationTime.Count(); i++)
            {
                CPUOccupationTime[i] = 0;
            }
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
            this.CPUAwaitTime = CPUAwaitTime;

            if (double.IsNaN(IOAwaitTime))
                this.IOAwaitTime = 0;
            else
                this.IOAwaitTime = IOAwaitTime;

            this.ProcessingTime = processingTime;

            UpdateResults();
        }

        public void CollectProcessor(double durration, int index)
        {
            CPUOccupationTime[index] += durration;
        }

        public void Finalization()
        {
            TermProcInTime = TerminatedProcessCount / ClockTime;

            AvrProcessingTime = 0;
            AvrCPUAwaitTime = 0;
            AvrIOAwaitTime = 0;
            AvrCPUOccupation = 0;
            foreach (var result in ResultsList)
            {
                AvrProcessingTime += result.ProcessingTime;
                AvrCPUAwaitTime += result.CPUAwaitTime;
                AvrIOAwaitTime += result.IOAwaitTime;

                foreach (var cpu in CPUOccupationTime)
                {
                    AvrCPUOccupation += cpu;
                }
                AvrCPUOccupation /= CPUOccupationTime.Count();
            }
            AvrProcessingTime /= TerminatedProcessCount;
            AvrCPUAwaitTime /= TerminatedProcessCount;
            AvrIOAwaitTime /= TerminatedProcessCount;
            AvrCPUOccupation /= TerminatedProcessCount;

        }
        #endregion
    }
}

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
        private int DisplayPoint { get; set; }
        private int AvrDepth { get; set; }
        //----------------------------------------------
        private double ProcessingTime { get; set; }
        private double CPUAwaitTime { get; set; }
        private double[] CPUOccupationTime { get; set; }
        private double IOAwaitTime { get; set; }
        //----------------------------------------------
        public double SumProcessingTime { get; set; }
        public double SumCPUAwaitTime { get; set; }
        public double[] SumCPUOccupationTime { get; set; }
        public double SumIOAwaitTime { get; set; }
        #endregion

        //------------------------------------------------------------------

        public List<Results> CurrentList { get; private set; } = new List<Results>();
        public List<Results> AverageList { get; private set; } = new List<Results>();
        private Queue<Results> TempList { get; set; } = new Queue<Results>();

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

        public ResultTracker(int DisplayPoint, int AvrDepth)
        { 
            this.DisplayPoint = DisplayPoint;
            this.AvrDepth = AvrDepth;
            ClockTime = 0;
            LastTime = 0;
            TerminatedProcessCount = 0;

        }

        private void UpdateCurrent()
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

            CurrentList.Add(_newResult);

            //Cleaning data before next check
            LastTime = ClockTime;
            for (int i = 0; i < CPUOccupationTime.Count(); i++)
            {
                CPUOccupationTime[i] = 0;
            }
        }

        private void UpdateSum()
        {
            var _newResult = new Results(CPUOccupationTime.Count());
            _newResult.TerminatedProcessCount = TerminatedProcessCount;
            if (ClockTime == 0)
                _newResult.TerminatedProcessesInTime = 0;
            else
                _newResult.TerminatedProcessesInTime = TerminatedProcessCount / ClockTime;
            _newResult.ProcessingTime = SumProcessingTime / TerminatedProcessCount;
            _newResult.CPUAwaitTime = SumCPUAwaitTime / TerminatedProcessCount;
            _newResult.IOAwaitTime = SumIOAwaitTime / TerminatedProcessCount;
            for (int i = 0; i < CPUOccupationTime.Count(); i++)
            {
                if (ClockTime == 0)
                    _newResult.CPUOccupation[i] = 0;
                else
                    _newResult.CPUOccupation[i] = CPUOccupationTime[i] / ClockTime;
            }

            AverageList.Add(_newResult);
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
            SumCPUOccupationTime = new double[numberOfCPUs];
            for (int i = 0; i < numberOfCPUs; i++)
            {
                SumCPUOccupationTime[i] = 0;
            }
        }

        public void CollectClockTime(double clockTime)
        {
            this.ClockTime = clockTime;
        }

        public void CollectProcess(double CPUAwaitTime, double IOAwaitTime, double processingTime)
        {
            this.TerminatedProcessCount++;
            //------------------------------------------
            this.CPUAwaitTime = CPUAwaitTime;
            this.SumCPUAwaitTime += CPUAwaitTime;

            if (double.IsNaN(IOAwaitTime))
            {
                this.IOAwaitTime = 0;
                this.SumIOAwaitTime += 0;
            }
            else
            {
                this.IOAwaitTime = IOAwaitTime;
                this.SumIOAwaitTime += IOAwaitTime;
            }

            this.ProcessingTime = processingTime;
            this.SumProcessingTime += processingTime;


            //------------------------------------------
            if (DisplayPoint <= TerminatedProcessCount)
            {
                UpdateCurrent();
                UpdateSum();
            }
        }

        public void CollectProcessor(double durration, int index)
        {
            CPUOccupationTime[index] += durration;
            SumCPUOccupationTime[index] += durration;
        }

        public void Finalization()
        {
            //TermProcInTime = TerminatedProcessCount / ClockTime;


        }
        #endregion
    }
}

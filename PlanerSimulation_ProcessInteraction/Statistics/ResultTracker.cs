using PlanerSimulation_ProcessInteraction.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PlanerSimulation_ProcessInteraction.Statistics
{
    class ResultTracker : IStatistics
    {
        #region Properties
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
        private int CurrentDepth { get; set; }
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

        #region Lists
        private Queue<Results> TempQueue { get; set; } = new Queue<Results>();
        public List<Results> CurrentList { get; private set; } = new List<Results>();
        public List<Results> AverageList { get; private set; } = new List<Results>();
        #endregion

        public struct Results
        {
            #region Properties
            public int TerminatedProcessCount { get; set; }
            public double TerminatedProcessesInTime { get; set; }
            public double ProcessingTime { get; set; }
            public double CPUAwaitTime { get; set; }
            public double IOAwaitTime { get; set; }
            public double[] CPUOccupation { get; set; }
            #endregion

            public Results(int numberOfCPUs, int terminatedProcessCount)
            {
                TerminatedProcessCount = terminatedProcessCount;
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

            public Results(int numberOfCPUs) : this(numberOfCPUs, 0)
            { }

            #region Operators
            public static Results operator +(Results r1, Results r2)
            {
                if (r1.CPUOccupation.Count() != r2.CPUOccupation.Count())
                    throw new System.ArgumentException("Two results must have the same lenght of CPUOccupation table");

                var _temp = new Results(r1.CPUOccupation.Count());
                //_temp.TerminatedProcessCount = r1.TerminatedProcessCount + r2.TerminatedProcessCount;
                _temp.TerminatedProcessCount = r1.TerminatedProcessCount;
                _temp.TerminatedProcessesInTime = r1.TerminatedProcessesInTime + r2.TerminatedProcessesInTime;
                _temp.ProcessingTime = r1.ProcessingTime + r2.ProcessingTime;
                _temp.CPUAwaitTime = r1.CPUAwaitTime + r2.CPUAwaitTime;
                _temp.IOAwaitTime = r1.IOAwaitTime + r2.IOAwaitTime;
                for (int i = 0; i < r1.CPUOccupation.Count(); i++)
                {
                    _temp.CPUOccupation[i] = r1.CPUOccupation[i] + r2.CPUOccupation[i];
                }

                return _temp;
            }
            public static Results operator /(Results r, int dev)
            {
                var _temp = new Results(r.CPUOccupation.Count());
                //_temp.TerminatedProcessCount = r.TerminatedProcessCount / dev;
                _temp.TerminatedProcessCount = r.TerminatedProcessCount;
                _temp.TerminatedProcessesInTime = r.TerminatedProcessesInTime / dev;
                _temp.ProcessingTime = r.ProcessingTime / dev;
                _temp.CPUAwaitTime = r.CPUAwaitTime / dev;
                _temp.IOAwaitTime = r.IOAwaitTime / dev;
                for (int i = 0; i < r.CPUOccupation.Count(); i++)
                {
                    _temp.CPUOccupation[i] = r.CPUOccupation[i] / dev;
                }

                return _temp;
            }
            public override string ToString()
            {
                var message = "Following Result contains:\n" +
                "TerminatedProcessCount = " + this.TerminatedProcessCount.ToString() + "\n" +
                "TerminatedProcessesInTime = " + this.TerminatedProcessesInTime.ToString() + "\n" +
                "ProcessingTime = " + this.ProcessingTime.ToString() + "\n" +
                "CPUAwaitTime = " + this.CPUAwaitTime.ToString() + "\n" +
                "IOAwaitTime = " + this.IOAwaitTime.ToString() + "\n";
                for (int i = 0; i < CPUOccupation.Count(); i++)
                {
                    message += "CPUOccupation" + i.ToString() + " = " + CPUOccupation[i].ToString() + "\n";
                }

                return message;
            }
            #endregion
        }

        public ResultTracker(int DisplayPoint, int CurrentDepth)
        { 
            this.DisplayPoint = DisplayPoint;
            this.CurrentDepth = CurrentDepth;
            ClockTime = 0;
            LastTime = 0;
            TerminatedProcessCount = 0;
        }

        private void UpdateCurrent()
        {
            var _newResult = new Results(CPUOccupationTime.Count());
            _newResult.TerminatedProcessCount = TerminatedProcessCount;
            //_newResult.TerminatedProcessesInTime = 1 / TimeSpan;
            _newResult.ProcessingTime = ProcessingTime;
            _newResult.CPUAwaitTime = CPUAwaitTime;
            _newResult.IOAwaitTime = IOAwaitTime;
            for (int i = 0; i < CPUOccupationTime.Count(); i++)
            {
                //_newResult.CPUOccupation[i] = CPUOccupationTime[i] / TimeSpan;
            }

            TempQueue.Enqueue(_newResult);
            TempQueue.Dequeue();
            if(TerminatedProcessCount >= 2*CurrentDepth + DisplayPoint)
            {
                var _temp = new Results(CPUOccupationTime.Count());
                _temp.TerminatedProcessCount = TerminatedProcessCount - CurrentDepth;
                foreach (var r in TempQueue)
                {
                    _temp += r;
                }
                _temp /= 2*CurrentDepth + 1;
                CurrentList.Add(_temp);
            }

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
                    _newResult.CPUOccupation[i] = SumCPUOccupationTime[i] / ClockTime;
            }

            AverageList.Add(_newResult);
        }

        public Results GetAverage()
        {
            var _temp = new Results(CPUOccupationTime.Count());
            foreach (var r in CurrentList)
            {
                _temp += r;
            }
            _temp /= CurrentList.Count;
            return _temp;
        }

        //------------------------------------------------------------------

        #region IStatistics
        public void Initialization(Supervisor mySupervisor)
        {
            var numberOfCPUs = mySupervisor.MyCPUs.Count();
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

            for (int i = 0; i < 2*CurrentDepth + 1; i++)
            {
                TempQueue.Enqueue(new Results(numberOfCPUs));
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

        public void CollectCPU(double durration, int index)
        {
            CPUOccupationTime[index] += durration;
            SumCPUOccupationTime[index] += durration;
        }

        public void Finalization()
        {
            
        }
        #endregion
    }
}

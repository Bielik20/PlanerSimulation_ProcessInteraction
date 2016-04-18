using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PlanerSimulation_ProcessInteraction.Models;
using System.Windows;

namespace PlanerSimulation_ProcessInteraction.Statistics
{
    class OutcomeStats : IStatistics
    {
        public int TerminatedProcessCount { get; private set; }
        private double ProcessingTime { get; set; }
        private double CPUAwaitTime { get; set; }
        private double IOAwaitTime { get; set; }
        private double CPUOccupation { get; set; }

        private double StartClockTime { get; set; }
        private double EndClockTime { get; set; }
        private int NumOfCPUs { get; set; }
        private bool Flag { get; set; }

        private int StabilityPoint { get; set; }
        public Results MyResults { get; set; }


        public OutcomeStats(int stabilityPoint)
        {
            TerminatedProcessCount = 0;
            ProcessingTime = 0;
            CPUAwaitTime = 0;
            IOAwaitTime = 0;
            CPUOccupation = 0;
            Flag = false;
            this.StabilityPoint = stabilityPoint;
        }

        public struct Results
        {
            #region Properties
            public double TerminatedProcessesInTime { get; set; }
            public double ProcessingTime { get; set; }
            public double CPUAwaitTime { get; set; }
            public double IOAwaitTime { get; set; }
            public double CPUOccupation { get; set; }
            #endregion

            public Results(double TerminatedProcessesInTime, double ProcessingTime, double CPUAwaitTime, double IOAwaitTime, double CPUOccupation)
            {
                this.TerminatedProcessesInTime = TerminatedProcessesInTime;
                this.ProcessingTime = ProcessingTime;
                this.CPUAwaitTime = CPUAwaitTime;
                this.IOAwaitTime = IOAwaitTime;
                this.CPUOccupation = CPUOccupation;
            }

            #region Operators
            public static Results operator +(Results r1, Results r2)
            {
                r1.TerminatedProcessesInTime += r2.TerminatedProcessesInTime;
                r1.ProcessingTime += r2.ProcessingTime;
                r1.CPUAwaitTime += r2.CPUAwaitTime;
                r1.IOAwaitTime += r2.IOAwaitTime;
                r1.CPUOccupation += r2.CPUOccupation;

                return r1;
            }
            public static Results operator /(Results r, int dev)
            {
                r.TerminatedProcessesInTime /= dev;
                r.ProcessingTime /= dev;
                r.CPUAwaitTime /= dev;
                r.IOAwaitTime /= dev;
                r.CPUOccupation /= dev;

                return r;
            }
            public override string ToString()
            {
                var message = "TerminatedProcessesInTime = " + TerminatedProcessesInTime + "\n" +
                    "ProcessingTime = " + ProcessingTime + "\n" +
                    "CPUAwaitTime = " + CPUAwaitTime + "\n" +
                    "IOAwaitTime = " + IOAwaitTime + "\n" +
                    "CPUOccupation = " + CPUOccupation;

                return message;
            }
            #endregion
        }

        public void CollectClockTime(double clockTime)
        {
            this.EndClockTime = clockTime;
        }

        public void CollectProcess(double CPUAwaitTime, double IOAwaitTime, double ProcessingTime)
        {
            TerminatedProcessCount++;
            if (TerminatedProcessCount > StabilityPoint && !Flag)
            {
                Flag = true;
                StartClockTime = EndClockTime;
            }
            if (Flag)
            {
                this.CPUAwaitTime += CPUAwaitTime;
                if (!double.IsNaN(IOAwaitTime))
                {
                    this.IOAwaitTime += IOAwaitTime;
                }
                this.ProcessingTime += ProcessingTime;
            }
        }

        public void CollectCPU(double durration, int index)
        {
            if (Flag)
                this.CPUOccupation += durration / NumOfCPUs;
        }

        public void Finalization()
        {
            var _terminatedProcessesInTime = (TerminatedProcessCount - StabilityPoint) / (EndClockTime - StartClockTime);
            var _processingTime = ProcessingTime / (TerminatedProcessCount - StabilityPoint);
            var _cpuAwaitTime = CPUAwaitTime / (TerminatedProcessCount - StabilityPoint);
            var _ioAwaitTime = IOAwaitTime / (TerminatedProcessCount - StabilityPoint);
            var _cpuOccupation = CPUOccupation / (EndClockTime - StartClockTime) * 100;

            MyResults = new Results(_terminatedProcessesInTime, _processingTime, _cpuAwaitTime, _ioAwaitTime, _cpuOccupation);
            //MessageBox.Show(MyResults.ToString());
        }

        public void Initialization(Supervisor mySupervisor)
        {
            NumOfCPUs = mySupervisor.MyCPUs.Count();
        }
    }
}

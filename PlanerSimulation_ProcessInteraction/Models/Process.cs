using PlanerSimulation_ProcessInteraction.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PlanerSimulation_ProcessInteraction.Models
{
    class Process
    {
        #region Basic
        private ProcessEvent MyEvent { get; set; }
        private Supervisor MySupervisor { get; set; }
        private CPU MyCPU { get; set; }
        private int MyIOIndex { get; set; }
        private Phase MyPhase { get; set; }
        private bool Terminated { get; set; }
        private bool Active { get; set; }
        private enum Phase
        {
            ProcessArrived,
            CPUAllocated,
            CPUInterrupted,
            IOAllocated,
            IOExecuted,
            Termination
        };
        #endregion

        #region Time Properties
        private double CPUTime { get; set; }
        private double CPUUsedTime { get; set; }
        private double IOTime { get; set; }
        private double WaitStart { get; set; } //Used to calculate AwaitTime
        private double AwaitTime { get { if (WaitStart < 0) throw new System.ArgumentException("Parameter cannot be negative", "AwaitTime"); return MySupervisor.ClockTime - WaitStart; } }
        private double CPURemainingTime { get { return CPUTime - CPUUsedTime; } }
        #endregion

        #region Statistics
        private double ArriveTime { get; set; }
        /// <summary>
        /// Whole time spent waiting for Processor.
        /// </summary>
        private double CPUWholeWaitTime
        {
            get { return _CPUWholeWaitTime; }
            set { _CPUWholeWaitTime = value; ProcessorAllocatedCount++; }
        }
        private double _CPUWholeWaitTime;
        private int ProcessorAllocatedCount { get; set; }
        /// <summary>
        /// Whole time spent waiting for IO Device.
        /// </summary>
        private double IOWholeWaitTime
        {
            get { return _IOWholeWaitTime; }
            set { _IOWholeWaitTime = value; IOAllocatedCount++; }
        }
        private double _IOWholeWaitTime;
        private int IOAllocatedCount { get; set; }
        #endregion

        #region Exposed Properties
        public double CPUPriority { get { return -CPURemainingTime + AwaitTime; } }
        public double IOPriority { get { return -IOTime + AwaitTime; } }
        #endregion


        public Process(Supervisor mySupervisor)
        {
            this.MySupervisor = mySupervisor;
            MyPhase = Phase.ProcessArrived;
            CPUUsedTime = 0;
            Terminated = false;
            MyEvent = new ProcessEvent(this);
        }


        public void Activate(double durration)
        {
            MyEvent.occurTime = MySupervisor.ClockTime + durration;
            MySupervisor.AddTimedEvent(MyEvent);
        }


        public void Execute()
        {
            if (Terminated == true)
                throw new System.ArgumentException("Parameter cannot be true", "Terminated");
            Active = true;

            while(Active)
            {
                switch(MyPhase)
                {
                    case Phase.ProcessArrived:
                        //Setting properties
                        ArriveTime = MySupervisor.ClockTime;
                        CPUTime = MySupervisor.RollEngine.ProcessorTime();
                        WaitStart = MySupervisor.ClockTime;

                        //Creating next process.
                        var newProcess = new Process(MySupervisor);
                        newProcess.Activate(MySupervisor.RollEngine.ArrivalTime());

                        //Placing self in queueA6
                        MySupervisor.AddA6(this);

                        //Checking if any processor is free. If true process will continue work.
                        MyPhase = Phase.CPUAllocated;
                        Active = false;
                        foreach (var processor in MySupervisor.MyCPUs)
                        {
                            Active |= processor.IsFree;
                        }
                        break;


                    case Phase.CPUAllocated:
                        //Setting properties
                        CPUWholeWaitTime += AwaitTime;
                        WaitStart = -1;

                        //Remove me from list then choose and occupy processor
                        MySupervisor.RemoveA6(this);
                        foreach (var _CPU in MySupervisor.MyCPUs)
                        {
                            if (_CPU.IsFree == true)
                            {
                                MyCPU = _CPU;
                                break;
                            }
                        }
                        MyCPU.Occupy();

                        //Checking time after which IO is requested. To make things simpler if it's below 1 it is considered that it has not been requested and termination begins.
                        var _IORequestTime = MySupervisor.RollEngine.IORequestTime(CPURemainingTime - 1);
                        if (_IORequestTime < 1)
                        {
                            Activate(CPURemainingTime);
                            MyPhase = Phase.Termination;
                            Active = false;
                            break;
                        }
                        else
                        {
                            Activate(_IORequestTime);
                            MyPhase = Phase.CPUInterrupted;
                            Active = false;
                            break;
                        }


                    case Phase.CPUInterrupted:
                        //Releases CPU and updates used time by time spend with cpu
                        CPUUsedTime += MyCPU.Release();

                        //Seting waitStart to calculate AwaitTime
                        WaitStart = MySupervisor.ClockTime;

                        //Roll IO number and place self in queueB4
                        MyIOIndex = MySupervisor.RollEngine.IODevice();
                        MySupervisor.AddB1(this, MyIOIndex);

                        //Setting Phase and active status.
                        MyPhase = Phase.IOAllocated;
                        Active = MySupervisor.MyIOs[MyIOIndex];
                        break;


                    case Phase.IOAllocated:
                        //Setting Properties
                        IOWholeWaitTime += AwaitTime;
                        WaitStart = -1;

                        //Remove from queueB4 and Occupy myIO
                        MySupervisor.RemoveB1(this, MyIOIndex);
                        MySupervisor.OccupyIO(MyIOIndex);

                        //Rolling time that process is going to spend with IO
                        IOTime = MySupervisor.RollEngine.IOTime();
                        Activate(IOTime);

                        //Setting Phase and active status.
                        MyPhase = Phase.IOExecuted;
                        Active = false;
                        break;


                    case Phase.IOExecuted: //MessageBox.Show("IOExecuted - " + ArriveTime.ToString() + "\nprocessorTime = " + processorTime.ToString() + "\nprocessorUsedTime = " + processorUsedTime.ToString());
                        //Setting Properties
                        WaitStart = MySupervisor.ClockTime;

                        //Placing self in queueA2 and ReleaseIO
                        MySupervisor.ReleaseIO(MyIOIndex);
                        MySupervisor.AddA6(this);

                        //Checking if any processor is free. If true process will continue work.
                        MyPhase = Phase.CPUAllocated;
                        Active = false;
                        foreach (var processor in MySupervisor.MyCPUs)
                        {
                            Active |= processor.IsFree;
                        }
                        break;


                    case Phase.Termination:
                        //All statistics summary should be done here
                        MySupervisor.MyStatistics.CollectProcess(CPUWholeWaitTime / ProcessorAllocatedCount, IOWholeWaitTime / IOAllocatedCount, MySupervisor.ClockTime - ArriveTime);
                        //MessageBox.Show("temination - " + ArriveTime.ToString() + "\n" + (ProcessorWholeWaitTime / ProcessorAllocatedCount).ToString() + "\n" + (IOWholeWaitTime / IOAllocatedCount).ToString() + "\n" + (mySupervisor.clockTime - ArriveTime).ToString());
                        MyCPU.Release();
                        Terminated = true;
                        Active = false;
                        break;
                }
            }
        }
    }
}

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
        private ProcessEvent myEvent { get; set; }
        private Supervisor mySupervisor { get; set; }
        private CPU myCPU { get; set; }
        private int myIOIndex { get; set; }
        private Phase myPhase { get; set; }
        private bool terminated { get; set; }
        private bool active { get; set; }
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
        private double waitStart { get; set; } //Used to calculate AwaitTime
        private double AwaitTime { get { if (waitStart < 0) throw new System.ArgumentException("Parameter cannot be negative", "AwaitTime"); return mySupervisor.clockTime - waitStart; } }
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
            this.mySupervisor = mySupervisor;
            myPhase = Phase.ProcessArrived;
            CPUUsedTime = 0;
            terminated = false;
            myEvent = new ProcessEvent(this);
        }


        public void Activate(double durration)
        {
            myEvent.occurTime = mySupervisor.clockTime + durration;
            mySupervisor.AddTimedEvent(myEvent);
        }


        public void Execute()
        {
            if (terminated == true)
                throw new System.ArgumentException("Parameter cannot be true", "Terminated");
            active = true;

            while(active)
            {
                switch(myPhase)
                {
                    case Phase.ProcessArrived:
                        //Setting properties
                        ArriveTime = mySupervisor.clockTime;
                        CPUTime = mySupervisor.rollEngine.ProcessorTime();
                        waitStart = mySupervisor.clockTime;

                        //Creating next process.
                        var newProcess = new Process(mySupervisor);
                        newProcess.Activate(mySupervisor.rollEngine.ArrivalTime());

                        //Placing self in queueA6
                        mySupervisor.AddA6(this);

                        //Checking if any processor is free. If true process will continue work.
                        myPhase = Phase.CPUAllocated;
                        active = false;
                        foreach (var processor in mySupervisor.myCPUs)
                        {
                            active |= processor.isFree;
                        }
                        break;


                    case Phase.CPUAllocated:
                        //Setting properties
                        CPUWholeWaitTime += AwaitTime;
                        waitStart = -1;

                        //Remove me from list then choose and occupy processor
                        mySupervisor.RemoveAX(this);
                        foreach (var _CPU in mySupervisor.myCPUs)
                        {
                            if (_CPU.isFree == true)
                            {
                                myCPU = _CPU;
                                break;
                            }
                        }
                        myCPU.Occupy();

                        //Checking time after which IO is requested. To make things simpler if it's below 1 it is considered that it has not been requested and termination begins.
                        var _IORequestTime = mySupervisor.rollEngine.IORequestTime(CPURemainingTime - 1);
                        if (_IORequestTime < 1)
                        {
                            Activate(CPURemainingTime);
                            myPhase = Phase.Termination;
                            active = false;
                            break;
                        }
                        else
                        {
                            Activate(_IORequestTime);
                            myPhase = Phase.CPUInterrupted;
                            active = false;
                            break;
                        }


                    case Phase.CPUInterrupted:
                        //Releases CPU and updates used time by time spend with cpu
                        CPUUsedTime += myCPU.Release();

                        //Seting waitStart to calculate AwaitTime
                        waitStart = mySupervisor.clockTime;

                        //Roll IO number and place self in queueB4
                        myIOIndex = mySupervisor.rollEngine.IODevice();
                        mySupervisor.AddB4(this, myIOIndex);

                        //Setting Phase and active status.
                        myPhase = Phase.IOAllocated;
                        active = mySupervisor.myIOs[myIOIndex];
                        break;


                    case Phase.IOAllocated:
                        //Setting Properties
                        IOWholeWaitTime += AwaitTime;
                        waitStart = -1;

                        //Remove from queueB4 and Occupy myIO
                        mySupervisor.RemoveB4(this, myIOIndex);
                        mySupervisor.OccupyIO(myIOIndex);

                        //Rolling time that process is going to spend with IO
                        IOTime = mySupervisor.rollEngine.IOTime();
                        Activate(IOTime);

                        //Setting Phase and active status.
                        myPhase = Phase.IOExecuted;
                        active = false;
                        break;


                    case Phase.IOExecuted: //MessageBox.Show("IOExecuted - " + ArriveTime.ToString() + "\nprocessorTime = " + processorTime.ToString() + "\nprocessorUsedTime = " + processorUsedTime.ToString());
                        //Setting Properties
                        waitStart = mySupervisor.clockTime;

                        //Placing self in queueA2 and ReleaseIO
                        mySupervisor.ReleaseIO(myIOIndex);
                        mySupervisor.AddA2(this);

                        //Checking if any processor is free. If true process will continue work.
                        myPhase = Phase.CPUAllocated;
                        active = false;
                        foreach (var processor in mySupervisor.myCPUs)
                        {
                            active |= processor.isFree;
                        }
                        break;


                    case Phase.Termination:
                        //All statistics summary should be done here
                        mySupervisor.myStatistics.CollectProcess(CPUWholeWaitTime / ProcessorAllocatedCount, IOWholeWaitTime / IOAllocatedCount, mySupervisor.clockTime - ArriveTime);
                        //MessageBox.Show("temination - " + ArriveTime.ToString() + "\n" + (ProcessorWholeWaitTime / ProcessorAllocatedCount).ToString() + "\n" + (IOWholeWaitTime / IOAllocatedCount).ToString() + "\n" + (mySupervisor.clockTime - ArriveTime).ToString());
                        myCPU.Release();
                        terminated = true;
                        active = false;
                        break;
                }
            }
        }
    }
}

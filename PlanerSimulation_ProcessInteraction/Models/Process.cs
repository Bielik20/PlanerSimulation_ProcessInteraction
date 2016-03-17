﻿using PlanerSimulation_ProcessInteraction.Helpers;
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
        private Processor myProcessor { get; set; }
        private int myIOIndex { get; set; }
        private Phase myPhase { get; set; }
        private bool terminated { get; set; }
        private bool active { get; set; }
        private enum Phase
        {
            ProcessArrived,
            CPUAllocated,
            TeskExecuted,
            IOAllocated,
            IOExecuted,
            Termination
        };
        #endregion

        #region Time Properties
        private double arriveTime { get; set; }
        private double processorTime { get; set; }
        private double processorUsedTime { get; set; }
        private double IOTime { get; set; }
        private double waitStart { get; set; } //Used to calculate AwaitTime
        private double AwaitTime { get { if (waitStart < 0) throw new System.ArgumentException("Parameter cannot be negative", "AwaitTime"); return mySupervisor.clockTime - waitStart; } }
        #endregion

        #region Exposed Properties
        public double ProcessorPriority { get { return -(processorTime - processorUsedTime) + AwaitTime; } }
        public double IOPriority { get { return -IOTime + AwaitTime; } }
        #endregion


        public Process(Supervisor mySupervisor)
        {
            this.mySupervisor = mySupervisor;
            myPhase = Phase.ProcessArrived;
            processorUsedTime = 0;
            terminated = false;
            myEvent = new ProcessEvent(this);
        }


        public void Activate(double durration)
        {
            myEvent.occurTime = mySupervisor.clockTime + durration;
            //MessageBox.Show("Activate - " + myPhase.ToString() + " - " + myEvent.occurTime.ToString());
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
                        //Setting variables
                        arriveTime = mySupervisor.clockTime;
                        MessageBox.Show("processArrived - " + arriveTime.ToString());
                        processorTime = mySupervisor.rollEngine.ProcessorTime();

                        //Creating next process
                        var newProcess = new Process(mySupervisor);
                        newProcess.Activate(mySupervisor.rollEngine.ArrivalTime());

                        //Placing self in queueA6
                        waitStart = mySupervisor.clockTime;
                        mySupervisor.AddA6(this);
                        myPhase = Phase.CPUAllocated;

                        //Checking if any processor is free. If true process will continue work.
                        active = false;
                        foreach (var processor in mySupervisor.myProcessors)
                        {
                            active |= processor.isFree;
                        }
                        break;

                    case Phase.CPUAllocated: //MessageBox.Show("CPUAllocated - " + arriveTime.ToString());
                        //Remove me from list then choose and occupy processor
                        mySupervisor.RemoveAX(this);
                        waitStart = -1;
                        foreach (var processor in mySupervisor.myProcessors)
                        {
                            if (processor.isFree == true)
                            {
                                myProcessor = processor;
                                break;
                            }
                        }
                        myProcessor.Occupy();

                        //Checking time after which IO is requested. To make things simpler if it's below 1 it is considered that it has not been requested and termination begins.
                        var _IORequestTime = mySupervisor.rollEngine.TaskTime(processorTime - processorUsedTime - 1);
                        MessageBox.Show("CPUAllocated - " + arriveTime.ToString() + "\nIORequestTime = " + _IORequestTime.ToString());
                        if (_IORequestTime < 1)
                        {
                            Activate(processorTime - processorUsedTime);
                            myPhase = Phase.Termination;
                            active = false;
                            break;
                        }
                        else
                        {
                            Activate(_IORequestTime);
                            myPhase = Phase.TeskExecuted;
                            active = false;
                            break;
                        }

                    case Phase.TeskExecuted: //MessageBox.Show("taskExecuted - " + arriveTime.ToString());
                        //Releases CPU and updates used time by time spend with cpu
                        processorUsedTime += myProcessor.Release();

                        //Seting waitStart to calculate AwaitTime
                        waitStart = mySupervisor.clockTime;

                        //Roll IO number and place self in queueB4
                        myIOIndex = mySupervisor.rollEngine.IODevice();
                        mySupervisor.AddB4(this, myIOIndex);

                        myPhase = Phase.IOAllocated;
                        active = mySupervisor.myIOs[myIOIndex];
                        break;

                    case Phase.IOAllocated: //MessageBox.Show("IOAllocated - " + arriveTime.ToString());
                        mySupervisor.RemoveB4(this, myIOIndex);
                        mySupervisor.OccupyIO(myIOIndex);
                        waitStart = -1;

                        IOTime = mySupervisor.rollEngine.IOTime();
                        Activate(IOTime);
                        myPhase = Phase.IOExecuted;
                        active = false;
                        break;

                    case Phase.IOExecuted: MessageBox.Show("IOExecuted - " + arriveTime.ToString() + "\nprocessorTime = " + processorTime.ToString() + "\nprocessorUsedTime = " + processorUsedTime.ToString());
                        mySupervisor.ReleaseIO(myIOIndex);

                        //Placing self in queueA2
                        mySupervisor.AddA2(this);
                        waitStart = mySupervisor.clockTime;
                        myPhase = Phase.CPUAllocated;

                        //Checking if any processor is free. If true process will continue work.
                        active = false;
                        foreach (var processor in mySupervisor.myProcessors)
                        {
                            active |= processor.isFree;
                        }
                        break;

                    case Phase.Termination: MessageBox.Show("temination - " + arriveTime.ToString());
                        //All statistics summary should be done here
                        myProcessor.Release();
                        terminated = true;
                        active = false;
                        break;
                }
            }
        }
    }
}

using PlanerSimulation_ProcessInteraction.Helpers;
using PlanerSimulation_ProcessInteraction.Statistics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PlanerSimulation_ProcessInteraction.Models
{
    class Supervisor
    {

        #region Lists of Events and Processes

        private List<ProcessEvent> timedEvents { get; set; } = new List<ProcessEvent>();
        private List<Process> queueA6 { get; set; } = new List<Process>();
        private List<Process> queueA2 { get; set; } = new List<Process>();
        private List<Process>[] queueB4 { get; set; }

        #endregion

        //----------------------------------

        #region Basic
        public double clockTime { get; private set; }
        private bool simulationTerminated { get; set; }
        public RollEngine rollEngine { get; private set; }
        public IStatistics myStatistics { get; private set; }
        public bool[] myIOs { get; private set; }
        public CPU[] myCPUs { get; private set; }
        #endregion

        //----------------------------------

        public Supervisor(int numOfCPUs, int numOfIOs, double L, IStatistics myStatistics)
        {
            clockTime = 0;
            simulationTerminated = false;
            rollEngine = new RollEngine(numOfIOs ,L);
            this.myStatistics = myStatistics;

            myCPUs = new CPU[numOfCPUs];
            for (int i = 0; i < myCPUs.Count(); i++)
                myCPUs[i] = new CPU(this, i);

            queueB4 = new List<Process>[numOfIOs];
            for (int i = 0; i < queueB4.Count(); i++)
                queueB4[i] = new List<Process>();

            myIOs = new bool[numOfIOs];
            for (int i = 0; i < myIOs.Count(); i++)
                myIOs[i] = true;
        }

        //----------------------------------

        /// <summary>
        /// Runs simulation with settings asigned in constructor. Can run only once per Supervisor.
        /// </summary>
        public void Simulate(int processesLimit)
        {
            if (simulationTerminated == true)
                throw new System.InvalidOperationException("You cannot run simulation second time with the same supervisor. Create new supervisor to run it again.");

            myStatistics.Initialization(this);
            var _current = new Process(this);
            _current.Activate(0);
            while (processesLimit > myStatistics.TerminatedProcessCount)
            {
                myStatistics.CollectClockTime(clockTime);

                _current = timedEvents[0].myProcess;
                clockTime = timedEvents[0].occurTime;
                timedEvents.RemoveAt(0);
                _current.Execute();
            }
            myStatistics.Finalization();
            simulationTerminated = true;
        }

        //----------------------------------

        #region List Methods

        public void AddTimedEvent(ProcessEvent processEvent)
        {
            timedEvents.Add(processEvent);
            timedEvents = timedEvents.OrderBy(x => x.occurTime).ToList();

            //MessageBox.Show("start of timedEvents");
            foreach (var _event in timedEvents)
            {
               // MessageBox.Show(_event.occurTime.ToString());
            }
            //MessageBox.Show("end of timedEvents");
        }

        public void AddA6(Process process)
        {
            queueA6.Add(process);
            queueA6 = queueA6.OrderByDescending(x => x.CPUPriority).ToList();
        }

        public void AddA2(Process process)
        {
            var index = rollEngine.FromRange(0, queueA2.Count);
            queueA2.Insert(index, process);
        }

        public void RemoveAX(Process process)
        {
            if (queueA2.Contains(process))
            {
                if (queueA2.IndexOf(process) != 0)
                    throw new System.ArgumentException("Parameter cannot be different than 0", "indexOfProcess");
                queueA2.Remove(process);
                return;
            }

            if (queueA6.Contains(process))
            {
                if (queueA6.IndexOf(process) != 0)
                    throw new System.ArgumentException("Parameter cannot be different than 0", "indexOfProcess");
                queueA6.Remove(process);
                return;
            }

            throw new System.ArgumentException("Process that was called to be executed wasn't on list", "process");
        }

        public void AddB4(Process process, int index)
        {
            queueB4[index].Add(process);
            queueB4[index] = queueB4[index].OrderByDescending(x => x.IOPriority).ToList();
        }

        public void RemoveB4(Process process, int index)
        {
            queueB4[index].Remove(process);
        }

        #endregion

        //----------------------------------

        #region Processor Methods
        /// <summary>
        /// Chooses list through roll and takes process to execute
        /// </summary>
        /// <param name="processor"></param>
        public void AllocateCPU(CPU processor)
        {
            var _choise = rollEngine.FromRange(0, 1);
            if (_choise == 0 && queueA2.Count != 0)
            {
                //queueA2[0].Execute();
                queueA2[0].Activate(0);
                return;
            }
            else if (queueA6.Count != 0)
            {
                //queueA6[0].Execute();
                queueA6[0].Activate(0);
                return;
            }
        }
        #endregion

        //----------------------------------

        #region IO Methods

        public void OccupyIO(int index)
        {
            myIOs[index] = false;
        }

        public void ReleaseIO(int index)
        {
            myIOs[index] = true;
            if (queueB4[index].Count > 0)
                //queueB4[index][0].Execute();
                queueB4[index][0].Activate(0);
        }

        #endregion
    }
}

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

        private List<ProcessEvent> TimedEvents { get; set; } = new List<ProcessEvent>();
        private List<Process> QueueA6 { get; set; } = new List<Process>();
        private Queue<Process>[] QueueB1 { get; set; }

        #endregion

        //----------------------------------

        #region Basic
        public double ClockTime { get; private set; }
        private bool SimulationTerminated { get; set; }
        public RollEngine RollEngine { get; private set; }
        public IStatistics MyStatistics { get; private set; }
        public bool[] MyIOs { get; private set; }
        public CPU[] MyCPUs { get; private set; }
        #endregion

        //----------------------------------

        public Supervisor(int numOfCPUs, int numOfIOs, double L, IStatistics myStatistics, double rollSeed)
        {
            ClockTime = 0;
            SimulationTerminated = false;
            RollEngine = new RollEngine(numOfIOs ,L ,rollSeed);
            this.MyStatistics = myStatistics;

            MyCPUs = new CPU[numOfCPUs];
            for (int i = 0; i < MyCPUs.Count(); i++)
                MyCPUs[i] = new CPU(this, i);

            QueueB1 = new Queue<Process>[numOfIOs];
            for (int i = 0; i < QueueB1.Count(); i++)
                QueueB1[i] = new Queue<Process>();

            MyIOs = new bool[numOfIOs];
            for (int i = 0; i < MyIOs.Count(); i++)
                MyIOs[i] = true;
        }

        //----------------------------------

        /// <summary>
        /// Runs simulation with settings asigned in constructor. Can run only once per Supervisor.
        /// </summary>
        public void Simulate(int processesLimit)
        {
            if (SimulationTerminated == true)
                throw new System.InvalidOperationException("You cannot run simulation second time with the same supervisor. Create new supervisor to run it again.");

            MyStatistics.Initialization(this);
            var _current = new Process(this);
            _current.Activate(0);
            while (processesLimit > MyStatistics.TerminatedProcessCount)
            {
                MyStatistics.CollectClockTime(ClockTime);

                _current = TimedEvents[0].myProcess;
                ClockTime = TimedEvents[0].occurTime;
                TimedEvents.RemoveAt(0);
                _current.Execute();
            }
            MyStatistics.Finalization();
            SimulationTerminated = true;
        }

        //----------------------------------

        #region List Methods

        public void AddTimedEvent(ProcessEvent processEvent)
        {
            TimedEvents.Add(processEvent);
            TimedEvents = TimedEvents.OrderBy(x => x.occurTime).ToList();

            //MessageBox.Show("start of timedEvents");
            foreach (var _event in TimedEvents)
            {
               // MessageBox.Show(_event.occurTime.ToString());
            }
            //MessageBox.Show("end of timedEvents");
        }

        public void AddA6(Process process)
        {
            QueueA6.Add(process);
            QueueA6 = QueueA6.OrderByDescending(x => x.CPUPriority).ToList();
        }

        public void RemoveA6(Process process)
        {
            if (QueueA6.Contains(process))
            {
                if (QueueA6.IndexOf(process) != 0)
                    throw new System.ArgumentException("Parameter cannot be different than 0", "indexOfProcess");
                QueueA6.Remove(process);
                return;
            }

            throw new System.ArgumentException("Process that was called to be executed wasn't on list", "process");
        }

        public void AddB1(Process process, int index)
        {
            QueueB1[index].Enqueue(process);
        }

        public void RemoveB1(Process process, int index)
        {
            if (process != QueueB1[index].Dequeue())
                throw new System.ArgumentException("Must dequeue processes that is calling this methos");
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
            if (QueueA6.Count != 0)
            {
                QueueA6[0].Activate(0);
                return;
            }
        }
        #endregion

        //----------------------------------

        #region IO Methods

        public void OccupyIO(int index)
        {
            MyIOs[index] = false;
        }

        public void ReleaseIO(int index)
        {
            MyIOs[index] = true;
            if (QueueB1[index].Count > 0)
                QueueB1[index].First().Activate(0);
        }

        #endregion
    }
}

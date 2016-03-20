using PlanerSimulation_ProcessInteraction.Models;
using PlanerSimulation_ProcessInteraction.Statistics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace PlanerSimulation_ProcessInteraction.ViewModels
{
    class DebugViewModel : ViewModelBase
    {
        #region Commands
        public ICommand SimulateCommand { get; private set; }
        #endregion

        public SimResults Stats { get; set; }
        //public ResultTracker Stats { get; set; }

        public DebugViewModel()
        {
            SimulateCommand = new RelayCommand(_ => Simulate());
        }

        public void Simulate()
        {
            var numOfIOs = 5;
            var numOfCPUs = 2;
            var L = 0.05;
            var stabilityPoint = 0;
            var processesLimit = 300;

            Stats = new SimResults(stabilityPoint);
            //Stats = new ResultTracker(stabilityPoint);
            var _supervisor = new Supervisor(numOfIOs, numOfCPUs, L, Stats);
            _supervisor.Simulate(processesLimit);

            #region OLD - to delete
            /*
            var message = "End Results of simulation:\n" +
                "terminatedProcessCount = " + Stats.ResultsList.Last().terminatedProcessCount.ToString() + "\n" +
                "terminatedProcessInTime = " + Stats.ResultsList.Last().terminatedProcessesInTime.ToString() + "\n" +
                "avrCPUAwaitTime = " + Stats.ResultsList.Last().avrCPUAwaitTime.ToString() + "\n" +
                "avrIOAwaitTime = " + Stats.ResultsList.Last().avrIOAwaitTime.ToString() + "\n" +
                "avrProcessingTime = " + Stats.ResultsList.Last().avrProcessingTime.ToString() + "\n" +
                "avrCPUOccupation1 = " + Stats.ResultsList.Last().avrCPUOccupation[0].ToString() + "\n" +
                "avrCPUOccupation2 = " + Stats.ResultsList.Last().avrCPUOccupation[1].ToString() + "\n";

            var message2 = "TermProcInTime = " + Stats.TermProcInTime.ToString() + "\n" +
                "AvrProcessingTime = " + Stats.AvrProcessingTime.ToString() + "\n" +
                "AvrCPUAwaitTime = " + Stats.AvrCPUAwaitTime.ToString() + "\n" +
                "AvrIOAwaitTime = " + Stats.AvrIOAwaitTime.ToString() + "\n" +
                "AvrCPUOccupation = " + Stats.AvrCPUOccupation.ToString();
                
            MessageBox.Show(Stats.ClockTime.ToString());
            MessageBox.Show(message2);
            */
            OnPropertyChanged("Stats");
            #endregion
        }
    }
}

using PlanerSimulation_ProcessInteraction.Models;
using PlanerSimulation_ProcessInteraction.Statistics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PlanerSimulation_ProcessInteraction.ViewModels
{
    class MainWindowViewModel : ViewModelBase
    {
        public SimResults Stats { get; set; }
        public MainWindowViewModel()
        {
            var numOfIOs = 5;
            var numOfCPUs = 2;
            var L = 0.05;

            Stats = new SimResults(numOfCPUs);
            var _supervisor = new Supervisor(numOfIOs, numOfCPUs, L, Stats);
            _supervisor.Simulate();

            #region OLD - to delete
            var message = "End Results of simulation:\n" +
                "terminatedProcessCount = " + Stats.ResultsList.Last().terminatedProcessCount.ToString() + "\n" +
                "terminatedProcessInTime = " + Stats.ResultsList.Last().terminatedProcessesInTime.ToString() + "\n" +
                "avrCPUAwaitTime = " + Stats.ResultsList.Last().avrCPUAwaitTime.ToString() + "\n" +
                "avrIOAwaitTime = " + Stats.ResultsList.Last().avrIOAwaitTime.ToString() + "\n" +
                "avrProcessingTime = " + Stats.ResultsList.Last().avrProcessingTime.ToString() + "\n" +
                "avrCPUOccupation1 = " + Stats.ResultsList.Last().avrCPUOccupation[0].ToString() + "\n" +
                "avrCPUOccupation2 = " + Stats.ResultsList.Last().avrCPUOccupation[1].ToString() + "\n";

            //MessageBox.Show(Stats.ClockTime.ToString());
            MessageBox.Show(message);
            //OnPropertyChanged("Stats");
            #endregion
        }
    }
}

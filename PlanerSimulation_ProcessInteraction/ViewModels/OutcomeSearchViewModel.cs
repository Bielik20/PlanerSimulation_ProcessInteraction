using MyUtilities;
using PlanerSimulation_ProcessInteraction.Models;
using PlanerSimulation_ProcessInteraction.Statistics;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PlanerSimulation_ProcessInteraction.ViewModels
{
    class OutcomeSearchViewModel : SimulationViewModelBase
    {
        NormalViewModel Overwatch { get; set; }
        public OutcomeStats.Results ResultsList { get; private set; }
        public ConcurrentBag<OutcomeStats.Results> AverageList { get; private set; }
        public OutcomeStats.Results ConfidenceInterval { get; private set; }

        public OutcomeSearchViewModel(NormalViewModel Overwatch)
        {
            this.Overwatch = Overwatch;
        }

        public override void Simulate()
        {
            ResultsList = new OutcomeStats.Results(0, 0, 0, 0, 0);
            AverageList = new ConcurrentBag<OutcomeStats.Results>();
            ConfidenceInterval = new OutcomeStats.Results(0, 0, 0, 0, 0);

            Parallel.For(0, Overwatch.NumOfTrials, i => RunSimulation(i));

            FindConfidenceInterval();
            OnPropertyChanged("ResultsList");
            OnPropertyChanged("ConfidenceInterval");
        }

        private void RunSimulation(int i)
        {
            var _stats = new OutcomeStats(Overwatch.StabilityPoint);
            var _supervisor = new Supervisor(Overwatch.NumOfCPUs, Overwatch.NumOfIOs, Overwatch.Lambda, _stats, RandomGenerator.SeedList[i]);
            _supervisor.Simulate(Overwatch.EndingPoint);
            UpdateList(_stats);
        }

        private void UpdateList(OutcomeStats _stats)
        {
            ResultsList += _stats.MyResults / Overwatch.NumOfTrials;
            AverageList.Add(_stats.MyResults);
        }

        private void FindConfidenceInterval()
        {
            var standardDeviation = new OutcomeStats.Results(0, 0, 0, 0, 0);
            foreach (var item in AverageList)
            {
                standardDeviation += (item - ResultsList) * (item - ResultsList);
            }
            standardDeviation.CPUAwaitTime = Math.Sqrt(standardDeviation.CPUAwaitTime / (Overwatch.NumOfTrials - 1));
            standardDeviation.CPUOccupation = Math.Sqrt(standardDeviation.CPUOccupation / (Overwatch.NumOfTrials - 1));
            standardDeviation.IOAwaitTime = Math.Sqrt(standardDeviation.IOAwaitTime / (Overwatch.NumOfTrials - 1));
            standardDeviation.ProcessingTime = Math.Sqrt(standardDeviation.ProcessingTime / (Overwatch.NumOfTrials - 1));
            standardDeviation.TerminatedProcessesInTime = Math.Sqrt(standardDeviation.TerminatedProcessesInTime / (Overwatch.NumOfTrials - 1));

            var chart = new System.Web.UI.DataVisualization.Charting.Chart();
            var tDistr = chart.DataManipulator.Statistics.InverseTDistribution(0.05, Overwatch.NumOfTrials - 1);
            
            ConfidenceInterval =  (standardDeviation * tDistr) / Math.Sqrt(Overwatch.NumOfTrials) ;
        }

    }
}

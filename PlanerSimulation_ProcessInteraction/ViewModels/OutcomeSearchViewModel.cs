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
    class OutcomeSearchViewModel : SimulationViewModelBase
    {
        NormalViewModel Overwatch { get; set; }
        public OutcomeStats.Results ResultsList { get; private set; }

        public OutcomeSearchViewModel(NormalViewModel Overwatch)
        {
            this.Overwatch = Overwatch;
        }

        public override void Simulate()
        {
            ResultsList = new OutcomeStats.Results(0, 0, 0, 0, 0);

            Parallel.For(0, Overwatch.NumOfTrials, _ => RunSimulation());

            OnPropertyChanged("ResultsList");
        }

        private void RunSimulation()
        {
            var _stats = new OutcomeStats(Overwatch.StabilityPoint);
            var _supervisor = new Supervisor(Overwatch.NumOfCPUs, Overwatch.NumOfIOs, Overwatch.Lambda, _stats, 0);
            _supervisor.Simulate(Overwatch.EndingPoint);
            UpdateList(_stats);
        }

        private void UpdateList(OutcomeStats _stats)
        {
            ResultsList += _stats.MyResults / Overwatch.NumOfTrials;
        }

    }
}

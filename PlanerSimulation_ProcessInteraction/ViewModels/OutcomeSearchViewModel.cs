using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanerSimulation_ProcessInteraction.ViewModels
{
    class OutcomeSearchViewModel : SimulationViewModelBase
    {
        NormalViewModel Overwatch { get; set; }

        public OutcomeSearchViewModel(NormalViewModel Overwatch)
        {
            this.Overwatch = Overwatch;
        }

        public override void Simulate()
        {
            throw new NotImplementedException();
        }
    }
}

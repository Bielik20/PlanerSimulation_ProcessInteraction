using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanerSimulation_ProcessInteraction.ViewModels
{
    class LSearchViewModel : SimulationViewModelBase
    {
        NormalViewModel Overwatch { get; set; }

        public LSearchViewModel(NormalViewModel Overwatch)
        {
            this.Overwatch = Overwatch;
        }

        public override void Simulate()
        {
            throw new NotImplementedException();
        }
    }
}

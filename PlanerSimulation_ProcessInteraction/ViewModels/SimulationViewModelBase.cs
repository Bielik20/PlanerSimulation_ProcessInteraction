using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanerSimulation_ProcessInteraction.ViewModels
{
    public abstract class SimulationViewModelBase : ViewModelBase
    {
        public abstract void Simulate();
    }
}

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanerSimulation_ProcessInteraction.ViewModels
{
    class SPSearchViewModel : ViewModelBase
    {
        NormalViewModel Overwatch { get; set; }

        public SPSearchViewModel(NormalViewModel Overwatch)
        {
            this.Overwatch = Overwatch;
        }
    }
}

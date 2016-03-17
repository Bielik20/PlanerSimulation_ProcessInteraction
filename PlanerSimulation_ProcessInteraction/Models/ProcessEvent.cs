using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanerSimulation_ProcessInteraction.Models
{
    class ProcessEvent
    {
        public double occurTime { get; set; }
        public Process myProcess { get; set; }

        public ProcessEvent(Process myProcess)
        {
            this.myProcess = myProcess;
        }
    }
}

using PlanerSimulation_ProcessInteraction.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanerSimulation_ProcessInteraction.Helpers
{
    class CPU
    {
        #region Initialization
        public bool IsFree { get; private set; }
        private Supervisor MySupervisor { get; set; }
        private int MyIndex { get; set; }

        public CPU(Supervisor mySupervisor, int myIndex)
        {
            this.MySupervisor = mySupervisor;
            this.MyIndex = myIndex;
            IsFree = true;
        }
        #endregion

        #region Statistics
        private double OccupationStart { get; set; }
        #endregion

        public void Occupy()
        {
            if (IsFree == false)
                throw new System.ArgumentException("Parameter cannot be false if trying to occupy CPU", "isFree");

            IsFree = false;
            OccupationStart = MySupervisor.ClockTime;
        }

        public double Release()
        {
            if(IsFree == true)
                throw new System.ArgumentException("Parameter cannot be true if trying to release CPU", "isFree");
            IsFree = true;

            var durration = MySupervisor.ClockTime - OccupationStart;
            MySupervisor.MyStatistics.CollectCPU(durration, MyIndex);
            //occupationTime += durration; //Shouldn't be necessery since introducing myStatistics

            MySupervisor.AllocateCPU(this);

            return durration;
        }
    }
}

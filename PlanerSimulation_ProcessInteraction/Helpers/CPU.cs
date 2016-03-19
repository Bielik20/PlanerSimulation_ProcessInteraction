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
        public bool isFree { get; private set; }
        private Supervisor mySupervisor { get; set; }
        private int myIndex { get; set; }

        public CPU(Supervisor mySupervisor, int myIndex)
        {
            this.mySupervisor = mySupervisor;
            this.myIndex = myIndex;
            isFree = true;
        }
        #endregion

        #region Statistics
        private double occupationStart { get; set; }
        private double occupationTime { get; set; }
        #endregion

        public void Occupy()
        {
            if (isFree == false)
                throw new System.ArgumentException("Parameter cannot be false if trying to occupy CPU", "isFree");

            isFree = false;
            occupationStart = mySupervisor.clockTime;
        }

        public double Release()
        {
            if(isFree == true)
                throw new System.ArgumentException("Parameter cannot be true if trying to release CPU", "isFree");
            isFree = true;

            var durration = mySupervisor.clockTime - occupationStart;
            mySupervisor.myStatistics.CollectProcessor(durration, myIndex);
            //occupationTime += durration; //Shouldn't be necessery since introducing myStatistics

            mySupervisor.AllocateCPU(this);

            return durration;
        }
    }
}

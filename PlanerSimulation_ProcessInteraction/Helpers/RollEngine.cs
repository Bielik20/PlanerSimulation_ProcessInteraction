using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanerSimulation_ProcessInteraction.Helpers
{
    class RollEngine
    {
        private Random rnd;
        public RollEngine(double L)
        {
            this.L = L;
            rnd = new Random();
        }

        //-------------------------------------------------------------------------

        private double L; //arrivalTimeIntensity 
        /// <summary>
        /// Time span between arrivals of new processes.
        /// </summary>
        /// <param name="L"></param>
        /// <returns></returns>
        public double ArrivalTime()
        {
            return rnd.NextDouble() * (2) + L - 1;     //just to make something out of this
        }

        //-------------------------------------------------------------------------

        private double processorTimeMin = 1;
        private double processorTimeMax = 50;
        /// <summary>
        /// TPW - time that process demands from CPU. It is whole time that process is going to spend with CPU.
        /// </summary>
        /// <param name="processorTimeMin"></param>
        /// <param name="processorTimeMax"></param>
        /// <returns></returns>
        public double ProcessorTime()    
        {
            return rnd.NextDouble() * (processorTimeMax - processorTimeMin) + processorTimeMin;
        }

        //-------------------------------------------------------------------------

        private double IORequestTimeMin = 0;
        /// <summary>
        /// TPIO - time that process is going to spend with CPU before requesting IO Device.
        /// </summary>
        /// <param name="IORequestTimeMin"></param>
        /// <param name="IORequestTimeMax"></param>
        /// <returns></returns>
        public double IORequestTime(double IORequestTimeMax)
        {
            return rnd.NextDouble() * (IORequestTimeMax - IORequestTimeMin) + IORequestTimeMin;
        }

        //-------------------------------------------------------------------------

        private double IOTimeMin = 1;
        private double IOTimeMax = 10;
        /// <summary>
        /// TPO - time that process is going to spend with IO.
        /// </summary>
        /// <param name="IOTimeMin"></param>
        /// <param name="IOTimeMax"></param>
        /// <returns></returns>
        public double IOTime()
        {
            return rnd.NextDouble() * (IOTimeMax - IOTimeMin) + IOTimeMin;
        }

        //-------------------------------------------------------------------------

        private int IODeviceMin = 0;
        private int IODeviceMax = 4;
        /// <summary>
        /// TPD - rolls number of IO device.
        /// </summary>
        /// <param name="IODeviceMin"></param>
        /// <param name="IODeviceMin"></param>
        /// <returns></returns>
        public int IODevice() 
        {
            return rnd.Next(IODeviceMin, IODeviceMax);
        }

        //-------------------------------------------------------------------------

        public int FromRange(int min, int max)
        {
            return rnd.Next(min, max);
        }


    }
}

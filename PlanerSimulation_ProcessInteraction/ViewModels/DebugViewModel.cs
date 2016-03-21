using PlanerSimulation_ProcessInteraction.Models;
using PlanerSimulation_ProcessInteraction.Statistics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace PlanerSimulation_ProcessInteraction.ViewModels
{
    class DebugViewModel : ViewModelBase
    {
        #region Commands
        public ICommand SimulateCommand { get; private set; }
        #endregion

        #region Display Properties
        public int NumOfCPUs
        {
            get { return _numOfCPUs; }
            set { _numOfCPUs = value; OnPropertyChanged("NumOfCPUs"); }
        }
        private int _numOfCPUs;

        public int NumOfIOs
        {
            get { return _numOfIOs; }
            set { _numOfIOs = value; OnPropertyChanged("NumOfIOs"); }
        }
        private int _numOfIOs;

        public int EndingPoint
        {
            get { return _endingPoint; }
            set { _endingPoint = value; OnPropertyChanged("EndingPoint"); }
        }
        private int _endingPoint;

        public int DisplayPoint
        {
            get { return _displayPoint; }
            set { _displayPoint = value; OnPropertyChanged("DisplayPoint"); }
        }
        private int _displayPoint;

        public double Lambda
        {
            get { return _lambda; }
            set { _lambda = value; OnPropertyChanged("Lambda"); }
        }
        private double _lambda;

        public int RollSeed
        {
            get { return _rollSeed; }
            set { _rollSeed = value; OnPropertyChanged("RollSeed"); }
        }
        private int _rollSeed;
        #endregion

        //public SimResults Stats { get; set; }
        public ResultTracker Stats { get; set; }

        public DebugViewModel()
        {
            SimulateCommand = new RelayCommand(_ => Simulate());
            NumOfCPUs = 2;
            NumOfIOs = 5;
            EndingPoint = 500;
            DisplayPoint = 0;
            Lambda = 0.05;
            RollSeed = 0; 
        }

        


        public void Simulate()
        {
            //Stats = new SimResults(DisplayPoint);
            Stats = new ResultTracker(DisplayPoint);
            var _supervisor = new Supervisor(NumOfCPUs, NumOfIOs, Lambda, Stats);
            _supervisor.Simulate(EndingPoint);

            #region OLD - to delete
            /*
            var message = "End Results of simulation:\n" +
                "terminatedProcessCount = " + Stats.ResultsList.Last().terminatedProcessCount.ToString() + "\n" +
                "terminatedProcessInTime = " + Stats.ResultsList.Last().terminatedProcessesInTime.ToString() + "\n" +
                "avrCPUAwaitTime = " + Stats.ResultsList.Last().avrCPUAwaitTime.ToString() + "\n" +
                "avrIOAwaitTime = " + Stats.ResultsList.Last().avrIOAwaitTime.ToString() + "\n" +
                "avrProcessingTime = " + Stats.ResultsList.Last().avrProcessingTime.ToString() + "\n" +
                "avrCPUOccupation1 = " + Stats.ResultsList.Last().avrCPUOccupation[0].ToString() + "\n" +
                "avrCPUOccupation2 = " + Stats.ResultsList.Last().avrCPUOccupation[1].ToString() + "\n";

            var message2 = "TermProcInTime = " + Stats.TermProcInTime.ToString() + "\n" +
                "AvrProcessingTime = " + Stats.AvrProcessingTime.ToString() + "\n" +
                "AvrCPUAwaitTime = " + Stats.AvrCPUAwaitTime.ToString() + "\n" +
                "AvrIOAwaitTime = " + Stats.AvrIOAwaitTime.ToString() + "\n" +
                "AvrCPUOccupation = " + Stats.AvrCPUOccupation.ToString();
                
            MessageBox.Show(Stats.ClockTime.ToString());
            MessageBox.Show(message2);
            */
            OnPropertyChanged("Stats");
            #endregion
        }
    }
}

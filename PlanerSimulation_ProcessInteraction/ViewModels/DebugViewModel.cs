using PlanerSimulation_ProcessInteraction.Models;
using PlanerSimulation_ProcessInteraction.Statistics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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

        public int CurrentDepth
        {
            get { return _currentDepth; }
            set { _currentDepth = value; OnPropertyChanged("CurrentDepth"); }
        }
        private int _currentDepth;

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

        public ResultTracker Stats { get; set; }
        private Thread SimulationThread { get; set; }

        public DebugViewModel()
        {
            NumOfCPUs = 2;
            NumOfIOs = 5;
            EndingPoint = 500;
            DisplayPoint = 0;
            CurrentDepth = 5;
            Lambda = 0.05;
            RollSeed = 0;
            SimulateCommand = new RelayCommand(_ => { SimulationThread = new Thread(Simulate); SimulationThread.IsBackground = true; SimulationThread.Start(); });
        }

        public void Simulate()
        {
            Stats = new ResultTracker(DisplayPoint, CurrentDepth);
            var _supervisor = new Supervisor(NumOfCPUs, NumOfIOs, Lambda, Stats);
            _supervisor.Simulate(EndingPoint);

            OnPropertyChanged("Stats");
            //MessageBox.Show(Stats.AverageList.Count.ToString());
            MessageBox.Show(Stats.GetAverage().ToString());
        }
    }
}

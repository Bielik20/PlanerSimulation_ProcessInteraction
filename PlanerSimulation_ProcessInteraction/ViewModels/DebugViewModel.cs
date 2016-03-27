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
            set
            {
                if (value < 1)
                    _displayPoint = 1;
                else
                    _displayPoint = value;
                OnPropertyChanged("DisplayPoint");
            }
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

        public int NumOfTrials
        {
            get { return _numOfTrials; }
            set { _numOfTrials = value; OnPropertyChanged("NumOfTrials"); }
        }
        private int _numOfTrials;

        #endregion

        public ResultTracker Stats { get; set; }
        private Thread SimulationThread { get; set; }
        public List<ResultTracker.Results> CurrentList { get; private set; }
        public List<ResultTracker.Results> AverageList { get; private set; }


        public DebugViewModel()
        {
            NumOfCPUs = 1;
            NumOfIOs = 10;
            EndingPoint = 500;
            DisplayPoint = 1;
            CurrentDepth = 0;
            Lambda = 0.01;
            RollSeed = 0;
            NumOfTrials = 50;
            SimulateCommand = new RelayCommand(_ => { SimulationThread = new Thread(Simulate); SimulationThread.IsBackground = true; SimulationThread.Start(); });
        }

        public void Simulate()
        {
            CreateLists();
            for (int i = 0; i < NumOfTrials; i++)
            {
                Stats = new ResultTracker(DisplayPoint, CurrentDepth);
                var _supervisor = new Supervisor(NumOfCPUs, NumOfIOs, Lambda, Stats, RollSeed);
                _supervisor.Simulate(EndingPoint);

                UpdateLists();
            }

            OnPropertyChanged("CurrentList");
            OnPropertyChanged("AverageList");
        }


        private void UpdateLists()
        {
            for (int i = 0; i < EndingPoint - DisplayPoint + 1 - 2 * CurrentDepth; i++)
            {
                CurrentList[i] += Stats.CurrentList[i] / NumOfTrials;
                AverageList[i] += Stats.AverageList[i] / NumOfTrials;
            }
        }

        private void CreateLists()
        {
            CurrentList = new List<ResultTracker.Results>();
            AverageList = new List<ResultTracker.Results>();

            for (int i = 0; i < EndingPoint - DisplayPoint + 1 - 2*CurrentDepth; i++)
            {
                var _result = new ResultTracker.Results(NumOfCPUs, i+1);
                CurrentList.Add(_result);
                AverageList.Add(_result);
            }
            MessageBox.Show(CurrentList.Count.ToString());
        }
    }
}

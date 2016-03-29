using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace PlanerSimulation_ProcessInteraction.ViewModels
{
    class NormalViewModel : ViewModelBase
    {
        #region Commands
        public ICommand DisplaySPCommand { get; private set; }
        public ICommand SimulateSPCommand { get; private set; }
        public ICommand DisplayLCommand { get; private set; }
        public ICommand SimulateLCommand { get; private set; }
        public ICommand DisplayOutcomeCommand { get; private set; }
        public ICommand SimulateOutcomeCommand { get; private set; }
        #endregion

        #region UI State Properties
        public SimulationViewModelBase CurrentStage
        {
            get { return _currentStage; }
            set { _currentStage = value; OnPropertyChanged("CurrentStage"); }
        }
        private SimulationViewModelBase _currentStage;

        public List<SimulationViewModelBase> PageViewModels
        {
            get { return _pageViewModels; }
            set { _pageViewModels = value; }
        }
        private List<SimulationViewModelBase> _pageViewModels;
        #endregion

        #region Display Properties
        public bool IsEnabled
        {
            get { return _isEnabled; }
            set { _isEnabled = value; OnPropertyChanged("IsEnabled"); }
        }
        private bool _isEnabled;

        public int StabilityPoint
        {
            get { return _stabilityPoint; }
            set { _stabilityPoint = value; OnPropertyChanged("StabilityPoint"); }
        }
        private int _stabilityPoint;

        public int EndingPoint
        {
            get { return _endingPoint; }
            set { _endingPoint = value; OnPropertyChanged("EndingPoint"); }
        }
        private int _endingPoint;

        public int NumOfTrials
        {
            get { return _numOfTrials; }
            set { _numOfTrials = value; OnPropertyChanged("NumOfTrials"); }
        }
        private int _numOfTrials;

        public double Lambda
        {
            get { return _lambda; }
            set { _lambda = Math.Round(value, 3); OnPropertyChanged("Lambda"); }
        }
        private double _lambda;

        public double LambdaSpan
        {
            get { return _lambdaSpan; }
            set { _lambdaSpan = Math.Round(value, 4); OnPropertyChanged("LambdaSpan"); }
        }
        private double _lambdaSpan;

        public int NumOfCPUs { get; private set; }
        public int NumOfIOs { get; private set; }
        public int NumOfLambdas { get; private set; }
        #endregion

        public NormalViewModel()
        {
            {
                PageViewModels = new List<SimulationViewModelBase>();
                PageViewModels.Add(new SPSearchViewModel(this));
                PageViewModels.Add(new LSearchViewModel(this));
                PageViewModels.Add(new OutcomeSearchViewModel(this));

                CurrentStage = PageViewModels[1];

                DisplaySPCommand = new RelayCommand(_ => CurrentStage = PageViewModels[0]);
                SimulateSPCommand = new RelayCommand(_ => SimulateSPSearch());
                DisplayLCommand = new RelayCommand(_ => CurrentStage = PageViewModels[1]);
                SimulateLCommand = new RelayCommand(_ => SimulateLSearch());
                DisplayOutcomeCommand = new RelayCommand(_ => CurrentStage = PageViewModels[2]);
                SimulateOutcomeCommand = new RelayCommand(_ => SimulateOutcomeSearch());

                IsEnabled = true;
                StabilityPoint = 100;
                EndingPoint = 400;
                NumOfTrials = 500;
                Lambda = 0.034;
                LambdaSpan = 0.0005;
                NumOfCPUs = 1;
                NumOfIOs = 10;
                NumOfLambdas = 5;
            }
        }


        public void SimulateSPSearch()
        {
            IsEnabled = false;
            MessageBox.Show("SP");
            CurrentStage = PageViewModels[0];
            CurrentStage.Simulate();
            IsEnabled = true;
            
        }

        public void SimulateLSearch()
        {
            MessageBox.Show("L");
            CurrentStage = PageViewModels[1];
        }

        public void SimulateOutcomeSearch()
        {
            MessageBox.Show("Outcome");
            CurrentStage = PageViewModels[2];
        }
    }
}

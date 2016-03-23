using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        #region UI Properties
        public ViewModelBase CurrentStage
        {
            get { return _currentStage; }
            set { _currentStage = value; OnPropertyChanged("CurrentStage"); }
        }
        private ViewModelBase _currentStage;

        public List<ViewModelBase> PageViewModels
        {
            get { return _pageViewModels; }
            set { _pageViewModels = value; }
        }
        private List<ViewModelBase> _pageViewModels;
        #endregion

        public NormalViewModel()
        {
            {
                PageViewModels = new List<ViewModelBase>();
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
            }
        }


        public void SimulateSPSearch()
        {
            MessageBox.Show("SP");
            CurrentStage = PageViewModels[0];
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

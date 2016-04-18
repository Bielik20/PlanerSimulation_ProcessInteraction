using PlanerSimulation_ProcessInteraction.Models;
using PlanerSimulation_ProcessInteraction.Statistics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using MyUtilities;

namespace PlanerSimulation_ProcessInteraction.ViewModels
{
    class MainWindowViewModel : ViewModelBase
    {
        #region Commands
        public ICommand NormalModeCommand { get; private set; }
        public ICommand DebugModeCommand { get; private set; }
        #endregion

        #region UI Properties
        public ViewModelBase CurrentMode
        {
            get { return _currentMode; }
            set { _currentMode = value; OnPropertyChanged("CurrentMode"); }
        }
        private ViewModelBase _currentMode;

        public List<ViewModelBase> PageViewModels
        {
            get { return _pageViewModels; }
            set { _pageViewModels = value; }
        }
        private List<ViewModelBase> _pageViewModels;
        #endregion

        public MainWindowViewModel()
        {
            PageViewModels = new List<ViewModelBase>();
            PageViewModels.Add(new NormalViewModel());
            PageViewModels.Add(new DebugViewModel());

            CurrentMode = PageViewModels[0];

            NormalModeCommand = new RelayCommand(_ => CurrentMode = PageViewModels[0]);
            DebugModeCommand = new RelayCommand(_ => CurrentMode = PageViewModels[1]);
        }
    }
}

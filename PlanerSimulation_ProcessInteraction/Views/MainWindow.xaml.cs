using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using PlanerSimulation_ProcessInteraction.Views;
using PlanerSimulation_ProcessInteraction.Models;
using PlanerSimulation_ProcessInteraction.Statistics;

namespace PlanerSimulation_ProcessInteraction.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            var _statistics = new SimResults(2);
            var _supervisor = new Supervisor(5, _statistics);
            _supervisor.Simulate();

            var message = "End Results of simulation:\n" +
                "terminatedProcessCount = " + _statistics.ResultsList.Last().terminatedProcessCount.ToString() + "\n" +
                "terminatedProcessInTime = " + _statistics.ResultsList.Last().terminatedProcessesInTime.ToString() + "\n" +
                "avrCPUAwaitTime = " + _statistics.ResultsList.Last().avrCPUAwaitTime.ToString() + "\n" +
                "avrIOAwaitTime = " + _statistics.ResultsList.Last().avrIOAwaitTime.ToString() + "\n" +
                "avrProcessingTime = " + _statistics.ResultsList.Last().avrProcessingTime.ToString() + "\n" +
                "avrCPUOccupation1 = " + _statistics.ResultsList.Last().avrCPUOccupation[0].ToString() + "\n" +
                "avrCPUOccupation2 = " + _statistics.ResultsList.Last().avrCPUOccupation[1].ToString() + "\n";

            MessageBox.Show(_statistics.ResultsList.Count().ToString());
            MessageBox.Show(message);
        }
    }
}

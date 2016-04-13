using PlanerSimulation_ProcessInteraction.Models;
using PlanerSimulation_ProcessInteraction.Statistics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PlanerSimulation_ProcessInteraction.ViewModels
{
    class LSearchViewModel : SimulationViewModelBase
    {
        NormalViewModel Overwatch { get; set; }
        public double[] Lambdas { get; private set; }
        public List<KeyVal<string,LStats.Results>> ResultsList { get; private set; }
        public double Size { get; set; } = 34;

        public class KeyVal<TKey, TVal>
        {
            public TKey Key { get; set; }
            public TVal Val { get; set; }

            public KeyVal(TKey Key, TVal Val)
            {
                this.Key = Key;
                this.Val = Val;
            }
        }

        public LSearchViewModel(NormalViewModel Overwatch)
        {
            this.Overwatch = Overwatch;
        }

        public override void Simulate()
        {
            Lambdas = new double[Overwatch.NumOfLambdas];
            ResultsList = new List<KeyVal<string, LStats.Results>>();
            Action[] mySimulations = new Action[Overwatch.NumOfLambdas];
            for (int i = 0; i < Overwatch.NumOfLambdas; i++)
            {
                Lambdas[i] = Math.Round(Overwatch.Lambda + (Overwatch.NumOfLambdas / 2 - i) * Overwatch.LambdaSpan, 9);
                CreateList(i);

                var _temp = i;
                mySimulations[i] = new Action(() => SetLambda(_temp));
            }
            Parallel.Invoke(mySimulations);
            OnPropertyChanged("ResultsList");
            OnPropertyChanged("Size");
        }

        private void SetLambda(int index)
        {
            Action[] mySimulations = new Action[Overwatch.NumOfTrials];
            for (int i = 0; i < Overwatch.NumOfTrials; i++)
            {
                mySimulations[i] = new Action(() => RunSimulation(index));
            }
            Parallel.Invoke(mySimulations);
        }

        private void RunSimulation(int index)
        {
            var _stats = new LStats(Overwatch.StabilityPoint);
            var _supervisor = new Supervisor(Overwatch.NumOfCPUs, Overwatch.NumOfIOs, Lambdas[index], _stats, 0);
            _supervisor.Simulate(Overwatch.EndingPoint);
            UpdateList(index, _stats);
        }

        private void UpdateList(int index, LStats _stats)
        {
            ResultsList[index].Val += _stats.MyResults / Overwatch.NumOfTrials;
        }

        private void CreateList(int index)
        {
            ResultsList.Add(new KeyVal<string, LStats.Results>(Lambdas[index].ToString() + "  ", new LStats.Results(0)));
        }
    }
}

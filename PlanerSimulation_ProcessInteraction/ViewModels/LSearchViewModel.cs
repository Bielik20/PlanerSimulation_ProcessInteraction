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
        public Action[] MySimulations { get; private set; }
        public List<KeyVal<double,LStats.Results>> ResultsList { get; private set; }

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
            ResultsList = new List<KeyVal<double, LStats.Results>>();
            MySimulations = new Action[Overwatch.NumOfLambdas];
            for (int i = 0; i < Overwatch.NumOfLambdas; i++)
            {
                Lambdas[i] = Math.Round(Overwatch.Lambda + (Overwatch.NumOfLambdas / 2 - i) * Overwatch.LambdaSpan, 5);
                CreateList(i);

                var _temp = i;
                MySimulations[i] = new Action(() => RunSimulation(_temp));
            }
            Parallel.Invoke(MySimulations);

            OnPropertyChanged("ResultsList");
        }

        private void RunSimulation(int index)
        {
            for (int i = 0; i < Overwatch.NumOfTrials; i++)
            {
                var _stats = new LStats(Overwatch.StabilityPoint);
                var _supervisor = new Supervisor(Overwatch.NumOfCPUs, Overwatch.NumOfIOs, Lambdas[index], _stats, 0);
                _supervisor.Simulate(Overwatch.EndingPoint);
                UpdateList(index, _stats);
            }
        }

        private void UpdateList(int index, LStats _stats)
        {
            ResultsList[index].Val += _stats.MyResults / Overwatch.NumOfTrials;
        }

        private void CreateList(int index)
        {
            ResultsList.Add(new KeyVal<double, LStats.Results>(Lambdas[index], new LStats.Results(0)));
        }
    }
}

using PlanerSimulation_ProcessInteraction.Models;
using PlanerSimulation_ProcessInteraction.Statistics;
using System;
using System.Collections.Concurrent;
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
        public KeyVal<double, LStats.Results>[] ResultsList { get; private set; }
        private ConcurrentBag<LStats.Results>[] AverageList { get; set; }
        public List<KeyVal<double, double>>[] ConfidenceInterval { get; set; }

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
            ResultsList = new KeyVal<double, LStats.Results>[Overwatch.NumOfLambdas];
            AverageList = new ConcurrentBag<LStats.Results>[Overwatch.NumOfLambdas];
            for (int i = 0; i < Overwatch.NumOfLambdas; i++)
            {
                Lambdas[i] = Math.Round(Overwatch.Lambda + (Overwatch.NumOfLambdas / 2 - i) * Overwatch.LambdaSpan, 9);
                CreateList(i);

                var _temp = i;
                Parallel.For(0, Overwatch.NumOfTrials, _ => RunSimulation(i));
            }
            FindConfidenceInterval();
            OnPropertyChanged("ResultsList");
            OnPropertyChanged("ConfidenceInterval");
            OnPropertyChanged("Lambdas");
        }

        private void SetLambda(int index)
        {
            Parallel.For(0, Overwatch.NumOfTrials, i => RunSimulation(index));
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
            AverageList[index].Add(_stats.MyResults);
        }

        private void CreateList(int index)
        {
            ResultsList[index] = new KeyVal<double, LStats.Results>(Lambdas[index], new LStats.Results(0));
            AverageList[index] = new ConcurrentBag<LStats.Results>();
        }

        private void FindConfidenceInterval()
        {
            var standardDeviation = new double[Overwatch.NumOfLambdas];
            for (int i = 0; i < Overwatch.NumOfLambdas; i++)
            {
                standardDeviation[i] = 0;
                foreach (var item in AverageList[i])
                {
                    standardDeviation[i] += Math.Pow(item.CPUAwaitTime - ResultsList[i].Val.CPUAwaitTime, 2);
                }
                standardDeviation[i] = Math.Sqrt(standardDeviation[i] / (Overwatch.NumOfTrials - 1));
            }

            ConfidenceInterval = new List<KeyVal<double, double>>[Overwatch.NumOfLambdas];
            for (int i = 0; i < Overwatch.NumOfLambdas; i++)
            {
                ConfidenceInterval[i] = new List<KeyVal<double, double>>();
                ConfidenceInterval[i].Add(new KeyVal<double, double>(Lambdas[i], 43.6));
                ConfidenceInterval[i].Add(new KeyVal<double, double>(Lambdas[i], 53.4));
            }
            ConfidenceInterval[2][1].Val = 40;

        }
    }
}

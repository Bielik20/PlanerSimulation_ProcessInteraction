﻿using PlanerSimulation_ProcessInteraction.Models;
using PlanerSimulation_ProcessInteraction.Statistics;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using MyUtilities;

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
            ConfidenceInterval = new List<KeyVal<double, double>>[Overwatch.NumOfLambdas];
            for (int index = 0; index < Overwatch.NumOfLambdas; index++)
            {
                Lambdas[index] = Math.Round(Overwatch.Lambda + (Overwatch.NumOfLambdas / 2 - index) * Overwatch.LambdaSpan, 9);
                CreateList(index);

                var _temp = index;
                Parallel.For(0, Overwatch.NumOfTrials, i => RunSimulation(index, i));
            }
            FindConfidenceInterval();
            OnPropertyChanged("ResultsList");
            OnPropertyChanged("ConfidenceInterval");
            OnPropertyChanged("Lambdas");
        }


        private void RunSimulation(int index, int i)
        {
            var _stats = new LStats(Overwatch.StabilityPoint);
            var _supervisor = new Supervisor(Overwatch.NumOfCPUs, Overwatch.NumOfIOs, Lambdas[index], _stats, RandomGenerator.SeedList[i]);
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
            ConfidenceInterval[index] = new List<KeyVal<double, double>>();
            ConfidenceInterval[index].Add(new KeyVal<double, double>(Lambdas[index], 0));
            ConfidenceInterval[index].Add(new KeyVal<double, double>(Lambdas[index], 0));
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

            var chart = new System.Web.UI.DataVisualization.Charting.Chart();
            //Linear Axis is other way around, it's simplest and easiest way to make i work... although it's werid
            for (int i = 0; i < Overwatch.NumOfLambdas; i++)
            {
                var tDistr = chart.DataManipulator.Statistics.TDistribution(standardDeviation[i] / Math.Sqrt(Overwatch.NumOfTrials), 5, true);
                ConfidenceInterval[i][0].Val = ResultsList[Overwatch.NumOfLambdas - 1 - i].Val.CPUAwaitTime - tDistr;
                ConfidenceInterval[i][1].Val = ResultsList[Overwatch.NumOfLambdas - 1 - i].Val.CPUAwaitTime + tDistr;
            }
        }

    }
}

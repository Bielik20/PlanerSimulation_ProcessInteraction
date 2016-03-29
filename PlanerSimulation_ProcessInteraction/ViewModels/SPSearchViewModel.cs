﻿using PlanerSimulation_ProcessInteraction.Models;
using PlanerSimulation_ProcessInteraction.Statistics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PlanerSimulation_ProcessInteraction.ViewModels
{
    class SPSearchViewModel : SimulationViewModelBase
    {
        NormalViewModel Overwatch { get; set; }
        public double[] Lambdas { get; private set; }
        public Action[] MySimulations { get; set; }
        public List<SPStats.Results>[] ResultsList { get; private set; }

        public SPSearchViewModel(NormalViewModel Overwatch)
        {
            this.Overwatch = Overwatch;
        }

        public override void Simulate()
        {
            Lambdas = new double[Overwatch.NumOfLambdas];
            ResultsList = new List<SPStats.Results>[Overwatch.NumOfLambdas];
            MySimulations = new Action[Overwatch.NumOfLambdas];
            for (int i = 0; i < Overwatch.NumOfLambdas; i++)
            {
                Lambdas[i] = Math.Round(Overwatch.Lambda + (Overwatch.NumOfLambdas/2 - i) * Overwatch.LambdaSpan, 5);
                CreateList(i);

                var _temp = i;
                MySimulations[i] = new Action(() => RunSimulation(_temp));
            }
            Parallel.Invoke(MySimulations);

            OnPropertyChanged("ResultsList");
            OnPropertyChanged("Lambdas");
        }

        private void RunSimulation(int index)
        {
            for (int i = 0; i < Overwatch.NumOfTrials; i++)
            {
                var _stats = new SPStats();
                var _supervisor = new Supervisor(Overwatch.NumOfCPUs, Overwatch.NumOfIOs, Lambdas[index], _stats, 0);
                _supervisor.Simulate(Overwatch.EndingPoint);
                UpdateList(index, _stats);
            }
        }

        private void CreateList(int index)
        {
            ResultsList[index] = new List<SPStats.Results>();
            for (int i = 0; i < Overwatch.EndingPoint; i++)
            {
                ResultsList[index].Add(new SPStats.Results(i + 1, 0));
            }
        }

        private void UpdateList(int index, SPStats stats)
        {
            for (int i = 0; i < Overwatch.EndingPoint; i++)
            {
                ResultsList[index][i] += stats.ResultsList[i] / Overwatch.NumOfTrials;
            }
        }
    }
}

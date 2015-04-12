﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using MahApps.Metro.Controls;
using POESKillTree.SkillTreeFiles;
using POESKillTree.SkillTreeFiles.SteinerTrees;
using System.ComponentModel;
using MessageBox = POESKillTree.Views.MetroMessageBox;

namespace POESKillTree.Views
{
    /// <summary>
    /// Interaction logic for OptimizerController.xaml
    /// </summary>
    public partial class OptimizerControllerWindow : MetroWindow
    {
        private SteinerSolver steinerSolver;
        private SkillTree tree;
        private HashSet<ushort> targetNodes;

        private readonly BackgroundWorker solutionWorker = new BackgroundWorker();
        private readonly BackgroundWorker initializationWorker = new BackgroundWorker();

        int maxSteps;
        int step;

        public HashSet<ushort> bestSoFar;

        private bool isInitializing;
        private bool isPaused;
        private bool isCanceling;

        public OptimizerControllerWindow(SkillTree tree, HashSet<ushort> targetNodes)
        {
            InitializeComponent();
            this.tree = tree;
            steinerSolver = new SteinerSolver(tree);
            this.targetNodes = targetNodes;
            
            initializationWorker.DoWork += initializationWorker_DoWork;
            initializationWorker.RunWorkerCompleted += initializationWorker_RunWorkerCompleted;

            solutionWorker.DoWork += solutionWorker_DoWork;
            solutionWorker.ProgressChanged += solutionWorker_ProgressChanged;
            solutionWorker.RunWorkerCompleted += solutionWorker_RunWorkerCompleted;
            solutionWorker.WorkerReportsProgress = true;
            solutionWorker.WorkerSupportsCancellation = true;
        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            btnPopupCancelClose.Content = "Cancel";
            btnPopupPauseResume.Content = "Pause";

            initializationWorker.RunWorkerAsync();
        }


        void initializationWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            // This is also deferred to a background task as it might take a while.
            steinerSolver.InitializeSolver(targetNodes);
        }

        void initializationWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            maxSteps = steinerSolver.MaxGeneration;
            progressBar.Maximum = maxSteps;
            lblProgressText.Content = "0/" + maxSteps;
            isInitializing = false;

            isPaused = false;
            isCanceling = false;
            btnPopupCancelClose.IsEnabled = true;
            btnPopupPauseResume.IsEnabled = true;
            step = 0;
            solutionWorker.RunWorkerAsync();
        }

        void solutionWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = (BackgroundWorker)sender;
            while (!steinerSolver.IsConsideredDone)
            {
                steinerSolver.EvolutionStep();

                worker.ReportProgress(steinerSolver.CurrentGeneration, steinerSolver.BestSolution);

                if (worker.CancellationPending)
                    break;
            }
            e.Result = steinerSolver.BestSolution;
        }


        void solutionWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar.Value = e.ProgressPercentage;
            lblProgressText.Content = e.ProgressPercentage.ToString() + "/" + maxSteps;
            bestSoFar = (HashSet<ushort>)(e.UserState);
            lblBestResult.Content = "Best result so far: " + bestSoFar.Count +
                " additional points spent.";
            tree.HighlightedNodes = new HashSet<ushort>(bestSoFar.Concat(tree.SkilledNodes));
            tree.DrawNodeBaseSurroundHighlight();
        }

        void solutionWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (isCanceling)
            {
                btnPopupPauseResume.IsEnabled = true;
                isCanceling = false;
                return;
            }
            
            lblProgressText.Content = "Finished!";
            btnPopupCancelClose.Content = "Close";
            btnPopupPauseResume.IsEnabled = false;
            bestSoFar = (HashSet<ushort>)e.Result;
            isPaused = true;
        }

        #region UI interaction
        private void btnPopupCancelClose_Click(object sender, RoutedEventArgs e)
        {
            // Stop the optimizer and/or close the window.
            if (!isPaused)
            {
                solutionWorker.CancelAsync();
                isCanceling = true;
                DialogResult = false;
            }
            else
                DialogResult = true;
        }

        private void btnPopupPauseResume_Click(object sender, RoutedEventArgs e)
        {
            // Pause the optimizer
            if (isPaused)
            {
                btnPopupPauseResume.Content = "Pause";
                progressBar.IsEnabled = true;
                solutionWorker.RunWorkerAsync();
                isPaused = false;
            }
            else
            {
                btnPopupPauseResume.Content = "Continue";
                // Disable the button until the worker has actually finished.
                btnPopupPauseResume.IsEnabled = false;
                progressBar.IsEnabled = false;
                //lblProgressText.Content += " (paused)";
                solutionWorker.CancelAsync();
                isCanceling = true;
                isPaused = true;
            }
        }
        #endregion
    }
}

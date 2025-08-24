using System;
using System.ComponentModel;
using System.Threading;
using System.Windows;

namespace VisuALS_WPF_App
{
    class PeriodicBackgroundProcess
    {
        private BackgroundWorker periodicWorker = new BackgroundWorker();
        private FrameworkElement parentElement_;
        public int interval = 1000;
        public Action runFunction;
        public Action stoppedFunction;
        public Action startedFunction;
        public bool runOnLoad = true;

        public FrameworkElement ParentElement
        {
            get
            {
                return parentElement_;
            }
            set
            {
                parentElement_ = value;
                parentElement_.Loaded += ParentElement_Loaded;
                parentElement_.Unloaded += ParentElement_Unloaded;
            }
        }

        public bool isRunning = false;

        public PeriodicBackgroundProcess()
        {
            CommonInit();
        }

        public PeriodicBackgroundProcess(Action runfunc, int interval_ms = 1000, bool runOnParentLoaded = true)
        {
            interval = interval_ms;
            runFunction = runfunc;
            runOnLoad = runOnParentLoaded;
            CommonInit();
        }

        public PeriodicBackgroundProcess(Action runfunc, int interval_ms, FrameworkElement parentElement, bool runOnParentLoaded = true)
        {
            ParentElement = parentElement;
            interval = interval_ms;
            runFunction = runfunc;
            runOnLoad = runOnParentLoaded;
            CommonInit();
        }

        public PeriodicBackgroundProcess(Action startedFunc, Action runfunc, Action stoppedFunc, int interval_ms, FrameworkElement parentElement, bool runOnParentLoaded = true)
        {
            ParentElement = parentElement;
            interval = interval_ms;
            startedFunction = startedFunc;
            runFunction = runfunc;
            stoppedFunction = stoppedFunc;
            runOnLoad = runOnParentLoaded;
            CommonInit();
        }

        private void CommonInit()
        {
            periodicWorker.DoWork += periodicWorker_DoWork;
            periodicWorker.RunWorkerCompleted += PeriodicWorker_RunWorkerCompleted;
            if (runOnLoad)
            {
                StartProcess();
            }
        }

        private void ParentElement_Unloaded(object sender, RoutedEventArgs e)
        {
            StopProcess();
        }

        private void ParentElement_Loaded(object sender, RoutedEventArgs e)
        {
            if (runOnLoad)
            {
                StartProcess();
            }
        }

        public void StopProcess()
        {
            isRunning = false;
        }

        public void StartProcess()
        {
            if (!periodicWorker.IsBusy)
            {
                isRunning = true;
                periodicWorker.RunWorkerAsync();
            }
        }

        private void periodicWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            if (startedFunction != null)
            {
                startedFunction();
            }
            while (isRunning)
            {
                if (runFunction != null)
                {
                    runFunction();
                }
                Thread.Sleep(interval);
            }
        }

        private void PeriodicWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (stoppedFunction != null)
            {
                stoppedFunction();
            }
        }
    }
}

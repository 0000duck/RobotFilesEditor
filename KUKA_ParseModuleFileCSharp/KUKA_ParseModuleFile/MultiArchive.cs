using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ParseModuleFile
{
    class Opener
    {
        public string Path;
        public List<string> ExcludeList;
        public List<string> IncludeList;

        public Opener(string path, List<string> excludeList, List<string> includeList)
        {
            Path = path;
            ExcludeList = excludeList;
            IncludeList = includeList;
        }

        public Archive Open(System.ComponentModel.BackgroundWorker worker, System.ComponentModel.DoWorkEventArgs e)
        {
#if DEBUG
            return new Archive(Path, ExcludeList, IncludeList);
#else
            try
            {
                return new Archive(Path, ExcludeList, IncludeList);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debugger.Break();
                Exception w = ex;
                return null;
            }
#endif
        }
    }

    public class MultiArchive : NotifyPropertyChanged
    {
        private List<BackgroundWorker> workers = new List<BackgroundWorker>();
        private List<bool> workerDone = new List<bool>();
        private int threads;
        private string path;

        private ObservableCollection<Archive> archives = new ObservableCollection<Archive>();
        public ObservableCollection<Archive> Archives { get { return archives; } set { Set(ref archives, value); } }

        public MultiArchive(string path, int threads, List<string> excludeList, List<string> includeList)
        {
            this.threads = threads;
            this.path = path;

            InitMultiWorker();
            if (Directory.Exists(path))
            {
                string[] files = Directory.GetFiles(path, "*.zip", SearchOption.AllDirectories);
                Opener open;
                foreach (var file in files)
                {
                    open = new Opener(file, excludeList, includeList);
                    startWorker(open);
                }
                while (anyWorkersBusy())
                {
                    Thread.Sleep(100);
                }
                Console.WriteLine("Done");
            }
        }

        private void InitMultiWorker()
        {
            for (int i = 0; i < threads; i++)
            {
                BackgroundWorker worker = new BackgroundWorker();
                worker.DoWork += new System.ComponentModel.DoWorkEventHandler(myWorker_DoWork);
                worker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(myWorker_RunWorkerCompleted);
                workers.Add(worker);
                workerDone.Add(true);
            }
        }

        private void myWorker_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            System.ComponentModel.BackgroundWorker worker;
            worker = (System.ComponentModel.BackgroundWorker)sender;
            Opener opener = (Opener)e.Argument;
            e.Result = opener.Open(worker, e);
        }

        private void myWorker_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            System.ComponentModel.BackgroundWorker worker;
            worker = (System.ComponentModel.BackgroundWorker)sender;
            // Access the result through the Result property.
            Archive Area = (Archive)e.Result;
            int index = workers.IndexOf(worker);
            if (Area != null)
            {
                archives.Add(Area);
                Console.WriteLine("Done [" + (index + 1).ToString() + "]: " + Area.Robot.ToString());
            }
            workerDone[index] = true;
        }

        private bool allWorkersBusy()
        {
            foreach (bool done in workerDone)
            {
                if (done) return false;
            }
            return true;
        }

        private bool anyWorkersBusy()
        {
            foreach (bool done in workerDone)
            {
                if (!done) return true;
            }
            return false;
        }

        private void startWorker(Opener open)
        {
            while (allWorkersBusy())
            {
                Thread.Sleep(100);
            }
            for (int i = 0; i < threads; i++)
            {
                if (workerDone[i])
                {
                    workerDone[i] = false;
                    workers[i].RunWorkerAsync(open);
                    break;
                }
            }
        }
    }
}

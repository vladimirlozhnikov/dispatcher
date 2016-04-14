using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;

namespace Dispatcher.Business
{
    class JobsQuery
    {
        private Timer _timer;
        private readonly int _periodInMillis;
        private readonly List<Job> _jobs = new List<Job>();
        private readonly List<Job> _singleShotJobs = new List<Job>();
        private int _id = 1;
        private readonly List<int> _removeJobs = new List<int>(); 
        private readonly List<Job> _addedJobs = new List<Job>();

        public string Name { get; set; }

        public JobsQuery(int periodInMillis)
        {
            _periodInMillis = periodInMillis;
        }

        public Job AddJob()
        {
            lock (this)
            {
                Job job = new Job {Id = _id++};
                job.IntervalInMillis = _periodInMillis;
                _addedJobs.Add(job);

                return job;
            }
        }

        public Job FindByName(string nameJob)
        {
            lock (this)
            {
                Job j = _jobs.FirstOrDefault(j1 => string.Equals(j1.Name, nameJob));
                return j;
            }
        }

        public Job SingleShot()
        {
            lock (this)
            {
                Job job = new Job {Id = _id++};
                _singleShotJobs.Add(job);

                return job;
            }
        }

        public bool DeleteJob(Job job)
        {
            lock (this)
            {
                job.Ready = false;
                _removeJobs.Add(job.Id);
                return true;
            }
        }

        public void Run()
        {
            foreach (Job j in _jobs)
            {
                j.Ready = true;
            }

            _timer = new Timer(_periodInMillis);
            _timer.Elapsed += OnTimedEvent;
            _timer.AutoReset = true;
            _timer.Enabled = true;
        }

        public void Stop()
        {
            if (_timer == null)
                return;

            _timer.Stop();
            _timer.Close();
            _timer.Dispose();
            _timer = null;
            GC.Collect();

            _addedJobs.AddRange(_jobs);
            _jobs.Clear();
        }

        private void OnTimedEvent(object sender, ElapsedEventArgs e)
        {
            lock (this)
            {
                foreach (int i in _removeJobs)
                {
                    _jobs.RemoveAll(j => j.Id == i);
                }

                foreach (Job addedJob in _addedJobs)
                {
                    _jobs.Add(addedJob);
                }

                _addedJobs.Clear();

                foreach (Job j in _jobs)
                {
                    if (j.Ready)
                    {
                        j.Fire();
                    }
                }

                List<Job> tempsignleList = new List<Job>();
                foreach (Job j in _singleShotJobs)
                {
                    if (j.Ready)
                    {
                        if (j.Fire())
                        {
                            tempsignleList.Add(j);
                        }
                    }
                }

                foreach (Job j in tempsignleList)
                {
                    _singleShotJobs.Remove(j);
                }
                tempsignleList.Clear();
            }
        }
    }
}

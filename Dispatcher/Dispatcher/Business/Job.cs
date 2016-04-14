using System;

namespace Dispatcher.Business
{
    public class Job
    {
        public int Id { get; set; }
        public delegate TResult Func<in T, out TResult>(T arg);
        public Func<Job, bool> FuncEvent;
        private DateTime _lastdateTime = DateTime.Now;

        public long IntervalInMillis { get; set; }
        public object Tag { get; set; }
        public string Name { get; set; }
        public bool Ready { get; set; }

        public bool Fire()
        {
            if (IntervalInMillis == 0)
                return true;

            DateTime now = DateTime.Now;
            TimeSpan sp = now - _lastdateTime;
            if (Math.Abs(sp.TotalMilliseconds) > IntervalInMillis)
            {
                if (FuncEvent != null)
                {
                    FuncEvent(this);
                }

                _lastdateTime = now;
                return true;
            }

            return false;
        }
    }
}

using System;
using System.Threading;

namespace TimesheetTracker
{
    public class Debouncer
    {
        private readonly Timer _timer;
        private readonly int _delayMilliseconds;
        private Action _action;

        public Debouncer(int delayMilliseconds)
        {
            _delayMilliseconds = delayMilliseconds;
            _timer = new Timer(Execute, null, Timeout.Infinite, Timeout.Infinite);
        }

        public void Debounce(Action action)
        {
            _action = action;
            _timer.Change(_delayMilliseconds, Timeout.Infinite);
        }

        private void Execute(object state)
        {
            _action?.Invoke();
            _action = null;
        }
    }
}

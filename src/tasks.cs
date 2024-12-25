// Copyright (C) 2024 Tycho Softworks.
// This code is licensed under MIT license.

using System.Threading;

namespace Tychosoft.Extensions {
    public class Tasks {
        private readonly object mutex = new();
        private readonly Thread thread;
        private readonly Deque<Tuple<Action<object[]>, object[]>> tasks = new();
        private bool running = false;
	    private Func<TimeSpan> timeoutStrategy;
        private Action? shutdownStrategy = null;
        private Action<Exception>? errorHandler = null;

        // Default constructor with default timeout strategy
        public Tasks() : this(DefaultTimeoutStrategy, null) {}

	    public Tasks(Func<TimeSpan> timeout, Action? shutdown) {
		    timeoutStrategy = timeout ?? DefaultTimeoutStrategy;
            shutdownStrategy = shutdown;
            thread = new Thread(Process);
	    }

        public bool LimitedDispatch(int limit, Action<object[]> action, params object[] args) {
            lock (mutex) {
                if(!running || tasks.Count >= limit)
                    return false;

                tasks.PutBack(Tuple.Create(action, args));
                Monitor.Pulse(mutex);
            }
            return true;
        }

        public bool PriorityDispatch(Action<object[]> action, params object[] args) {
            lock (mutex) {
                if(!running)
                    return false;
                tasks.PutFront(Tuple.Create(action, args));
                Monitor.Pulse(mutex);
            }
            return true;
        }

        public void Notify() {
            lock (mutex) {
                if(running)
                    Monitor.Pulse(mutex);
            }
        }

        public void Startup() {
            lock (mutex) {
                if(running)
                    return;
                running = true;
                thread.Start();
            }
        }

        public void Shutdown() {
            lock (mutex) {
                if(!running)
                    return;
                running = false;
                Monitor.PulseAll(mutex);
            }
            if(thread.IsAlive)
                thread.Join();
        }

        public void SetTimeout(Func<TimeSpan> timeout) {
            lock (mutex) {
                if(running) throw new InvalidOperationException("Tasks already running");
                timeoutStrategy = timeout;
            }
        }

        public void SetShitdown(Action shutdown) {
            lock (mutex) {
                if(running) throw new InvalidOperationException("Tasks already running");
                shutdownStrategy = shutdown;
            }
        }

        public void SetErrors(Action<Exception> errors) {
            lock (mutex) {
                if(running) throw new InvalidOperationException("Tasks already running");
                errorHandler = errors;
            }
        }

        public void Clear() {
            lock (mutex) {
                tasks.Clear();
            }
        }

        public bool IsEmpty() {
            lock (mutex) {
                return tasks.IsEmpty();
            }
        }

        public int Count() {
            lock (mutex) {
                return tasks.Count;
            }
        }

        private void Process() {
            try {
                Tuple<Action<object[]>, object[]> task;
                while(true) {
                    lock(mutex) {
                        if(!running)
                            return;

                        if(tasks.Count == 0) {
                            Monitor.Wait(mutex, timeoutStrategy());
                            continue;
                        }
                        task = tasks.GetFront();
                    }

                    var (action, args) = task;
                    try {
                        action(args);
                    }
                    catch(Exception ex) {
                        errorHandler?.Invoke(ex);
                    }
                }
            }
            finally {
                shutdownStrategy?.Invoke();
            }
        }

        private static TimeSpan DefaultTimeoutStrategy() {
            return TimeSpan.FromMinutes(1);
        }
    }
}

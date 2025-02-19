// Copyright (C) 2024 Tycho Softworks.
// This code is licensed under MIT license.

using System.Threading;

namespace Tychosoft.Extensions {
    public class TaskQueue {
        private readonly object sync = new();
        private readonly Thread thread;
        private readonly Deque<Tuple<Action<object[]>, object[]>> tasks = new();
        private bool running = false;
	    private Func<TimeSpan> timeoutStrategy;
        private Action? shutdownStrategy = null;
        private Action<Exception>? errorHandler = null;

        // Default constructor with default timeout strategy
        public TaskQueue() : this(DefaultTimeoutStrategy, null) {}

	    public TaskQueue(Func<TimeSpan> timeout, Action? shutdown) {
		    timeoutStrategy = timeout ?? DefaultTimeoutStrategy;
            shutdownStrategy = shutdown;
            thread = new Thread(Process);
	    }

        public bool Dispatch(Action<object[]> action, params object[] args) {
            lock (sync) {
                if(!running) return false;
                tasks.PutBack(Tuple.Create(action, args));
                Monitor.Pulse(sync);
            }
            return true;
        }

        public bool LimitedDispatch(int limit, Action<object[]> action, params object[] args) {
            lock (sync) {
                if(!running || tasks.Count >= limit) return false;
                tasks.PutBack(Tuple.Create(action, args));
                Monitor.Pulse(sync);
            }
            return true;
        }

        public bool PriorityDispatch(Action<object[]> action, params object[] args) {
            lock (sync) {
                if(!running) return false;
                tasks.PutFront(Tuple.Create(action, args));
                Monitor.Pulse(sync);
            }
            return true;
        }

        public void Notify() {
            lock (sync) {
                if(running)
                    Monitor.Pulse(sync);
            }
        }

        public void Startup() {
            lock (sync) {
                if(running) return;
                running = true;
                thread.Start();
            }
        }

        public void Shutdown() {
            lock (sync) {
                if(!running) return;
                running = false;
                Monitor.PulseAll(sync);
            }
            if(thread.IsAlive)
                thread.Join();
        }

        public void SetTimeout(Func<TimeSpan> timeout) {
            lock (sync) {
                if(running) throw new InvalidOperationException("Tasks already running");
                timeoutStrategy = timeout;
            }
        }

        public void SetShitdown(Action shutdown) {
            lock (sync) {
                if(running) throw new InvalidOperationException("Tasks already running");
                shutdownStrategy = shutdown;
            }
        }

        public void SetErrors(Action<Exception> errors) {
            lock (sync) {
                if(running) throw new InvalidOperationException("Tasks already running");
                errorHandler = errors;
            }
        }

        public void Clear() {
            lock (sync) {
                tasks.Clear();
            }
        }

        public bool IsEmpty() {
            lock (sync) {
                return tasks.IsEmpty();
            }
        }

        public int Count() {
            lock (sync) {
                return tasks.Count;
            }
        }

        private void Process() {
            try {
                Tuple<Action<object[]>, object[]> task;
                while (true) {
                    lock (sync) {
                        if (!running) return;
                        if (tasks.Count == 0) {
                            Monitor.Wait(sync, timeoutStrategy());
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

        static public T GetFuture<T>(Task<T> task, Func<bool> predicate, TimeSpan interval) {
            while (!task.Wait(interval)) {
                if (!predicate()) throw new OperationCanceledException();
            }
            return task.Result;
        }

        private static TimeSpan DefaultTimeoutStrategy() {
            return TimeSpan.FromMinutes(1);
        }
    }
}

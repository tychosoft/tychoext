// Copyright (C) 2024 Tycho Softworks.
// This code is licensed under MIT license.

using System.Collections.Concurrent;

namespace Tychosoft.Extensions {
    public class Tasks {
        private readonly ConcurrentQueue<Tuple<Action<object[]>, object[]>> tasks = new();
        private readonly AutoResetEvent events = new(false);
	    private readonly CancellationTokenSource cancellationTokenSource = new();
	    private readonly Task thread;
	    private Func<TimeSpan> timeoutStrategy;
        private Action? shutdownStrategy;
        private Action<Exception>? errorHandler;

        // Default constructor with default timeout strategy
        public Tasks() : this(DefaultTimeoutStrategy, null) {}

	    public Tasks(Func<TimeSpan> timeout, Action? shutdown) {
		    timeoutStrategy = timeout ?? DefaultTimeoutStrategy;
            shutdownStrategy = shutdown;
		    thread = Task.Run(() => Process(cancellationTokenSource.Token));
	    }

	    public void Dispatch(Action<object[]> action, params object[] args) {
            tasks.Enqueue(Tuple.Create(action, args));
            events.Set();
        }

        public void Notify() {
            events.Set();
        }

        public async Task ShutdownAsync() {
            cancellationTokenSource.Cancel();
            events.Set();
            await thread;
        }

        public void SetShutdown(Action handler) {
            Dispatch(args => {
                shutdownStrategy = (Action)args[0];
            }, handler);
        }

        public void SetTimeout(Func<TimeSpan> handler) {
            Dispatch(args => {
                timeoutStrategy = (Func<TimeSpan>)args[0];
            }, handler);
        }

        public void SetErrors(Action<Exception> handler) {
            errorHandler = handler;
        }

        private void Process(CancellationToken token) {
            bool used = false;
            while (!token.IsCancellationRequested) {
                while (tasks.TryDequeue(out var task)) {
                    var (action, args) = task;
                    try {
                        action(args);
                    }
                    catch (Exception ex) {
                        errorHandler?.Invoke(ex);
                    }
                    used = true;
                }

                if (!used) {
                    events.WaitOne(); // Infinite wait for first task added
                }
                else {
                    events.WaitOne(timeoutStrategy()); // Wait with the timeout strategy
                }                                                                           }

            if(used)
                shutdownStrategy?.Invoke(); // Call the shutdown strategy if provided
	    }

	    private static TimeSpan DefaultTimeoutStrategy() {
            return TimeSpan.FromMinutes(1);
        }
    }
} // end namespace

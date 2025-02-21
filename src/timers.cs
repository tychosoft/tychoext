// Copyright (C) 2024 Tycho Softworks.
// This code is licensed under MIT license.

namespace Tychosoft.Extensions {
public class TimerQueue {
    private readonly SortedDictionary<DateTime, List<(ulong, Action<object[]>, object[], TimeSpan)>> taskQueue = [];
    private readonly Timer timer;
    private readonly object sync = new();
    private ulong nextId = 1;
    private Action<Exception>? errorHandler;

    public TimerQueue() {
        timer = new Timer(Expire, null, Timeout.Infinite, Timeout.Infinite);
    }

    private ulong Dispatch(DateTime expires, ulong id, Action<object[]> task, object[] args, TimeSpan period = default) {
        if (!taskQueue.TryGetValue(expires, out var taskList)) {
            taskList = [];
            taskQueue[expires] = taskList;
        }

        taskList.Add((id, task, args, period));
        if (taskQueue.Count == 1 || expires < taskQueue.Keys.First()) {
            Scheduler();
        }
        return id;
    }

    public ulong At(DateTime expires, Action<object[]> task, params object[] args) {
        lock (sync) {
            return Dispatch(expires, nextId++, task, args);
        }
    }

    public ulong Periodic(TimeSpan period, Action<object[]> task, params object[] args) {
        var now = DateTime.Now;
        lock (sync) {
            return Dispatch(now.Add(period), nextId++, task, args, period);
        }
    }

    public void Cancel(ulong id) {
        lock (sync) {
            foreach (var key in taskQueue.Keys.ToList()) {
                var taskList = taskQueue[key];
                taskList.RemoveAll(t => t.Item1 == id);
                if (taskList.Count == 0)
                    taskQueue.Remove(key);
            }
            Scheduler();
        }
    }

    public DateTime Find(ulong id) {
        lock (sync) {
            foreach (var kvp in taskQueue) {
                if (kvp.Value.Any(t => t.Item1 == id)) return kvp.Key;
            }
            return DateTime.MinValue;
        }
    }

    public void SetErrors(Action<Exception> handler) {
        errorHandler = handler;
    }

    private void Expire(object? state) {
        List<(ulong, Action<object[]>, object[], TimeSpan)> runlist;
        var now = DateTime.Now;

        lock (sync) {
            if (taskQueue.Count == 0) return;
            var nextTime = taskQueue.Keys.First();
            if (nextTime > now) {
                Scheduler();
                return;
            }

            runlist = taskQueue[nextTime].Select(t => (t.Item1, t.Item2, t.Item3, t.Item4)).ToList();
            taskQueue.Remove(nextTime);
        }

        foreach (var (id, task, args, period) in runlist) {
            try {
                task(args);
                if (period > TimeSpan.Zero) {
                    lock (sync) {
                        Dispatch(now.Add(period), id, task, args, period);
                    }
                }
            }
            catch (Exception ex) {
                errorHandler?.Invoke(ex);
            }
        }

        lock (sync) {
            Scheduler();
        }
    }

    private void Scheduler() {
        if (taskQueue.Count == 0) {
            timer.Change(Timeout.Infinite, Timeout.Infinite);
        }
        else {
            var nextTime = taskQueue.Keys.First();
            var delay = (int)Math.Max((nextTime - DateTime.Now).TotalMilliseconds, 0);
            timer.Change(delay, Timeout.Infinite);
        }
    }
}
} // end namespace

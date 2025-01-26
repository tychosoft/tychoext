// Copyright (C) 2025 Tycho Softworks.
// This code is licensed under MIT license.

namespace Tychosoft.Extensions {
    public class SemaphoreCount(int limit) {
	private readonly object sync = new object();
	private int maxCount = limit;
	private int runCount = 0;
	private bool released = false;

	public void Wait() {
	    lock (sync) {
		++runCount;
		while (!released && runCount > maxCount) {
		    Monitor.Wait(sync);
		}

		if (released) {
		    --runCount;
		    throw new OperationCanceledException();
		}
	    }
	}

	public bool Wait(TimeSpan timeout) {
	    lock (sync) {
		++runCount;
		var start = DateTime.UtcNow;
		while (!released && runCount > maxCount) {
		    var remaining = timeout - (DateTime.UtcNow - start);
		    if (remaining <= TimeSpan.Zero || !Monitor.Wait(sync, remaining)) {
			--runCount;
			return false;
		    }
		}

		if (released) {
		    --runCount;
		    throw new OperationCanceledException();
		}
		return true;
	    }
	}

	public void Post() {
	    lock (sync) {
		if (runCount > 0) {
		    --runCount;
		    Monitor.Pulse(sync);
		}
	    }
	}

	public void Release() {
	    lock (sync) {
		released = true;
		Monitor.PulseAll(sync);
	    }
	}

	public void Reset(int limit) {
	    lock (sync) {
		released = false;
		maxCount = limit;
		Monitor.PulseAll(sync);
	    }
	}
    }
}

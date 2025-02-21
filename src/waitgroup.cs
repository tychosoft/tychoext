// Copyright (C) 2024 Tycho Softworks.
// This code is licensed under MIT license.

namespace Tychosoft.Extensions {
public class WaitGroup {
    private uint count = 0;
    private readonly SemaphoreSlim semaphore = new(0, int.MaxValue);

    public void Add(uint add = 1) {
        Interlocked.Add(ref count, add);
    }

    public void Done() {
        if (Interlocked.Decrement(ref count) == 0) {
            semaphore.Release();
        }
    }

    public uint Count() {
        return Volatile.Read(ref count);
    }

    public void Wait() {
        while (Volatile.Read(ref count) > 0) {
            semaphore.Wait();
        }
    }

    public async Task WaitAsync() {
        while (Volatile.Read(ref count) > 0) {
            await semaphore.WaitAsync();
        }
    }
}
} // end namespace


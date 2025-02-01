// Copyright (C) 2024 Tycho Softworks.
// This code is licensed under MIT license.

namespace Tychosoft.Extensions {
    public class AtomicInt(int initValue = 0) {
        private int value = initValue;
        public int Value => Get();

        public int Inc() {
            return Interlocked.Increment(ref value);
        }

        public int Dec() {
            return Interlocked.Decrement(ref value);
        }

        public int Get() {
            return value;
        }

        public int GetAndClear() {
            return Interlocked.CompareExchange(ref value, 0, 0);
        }

        public int Set(int newValue) {
            return Interlocked.Exchange(ref value, newValue);
        }
    }

    public class AtomicLong(long initValue = 0) {
        private long value = initValue;
        public long Value => Get();

        public long Inc() {
            return Interlocked.Increment(ref value);
        }

        public long Dec() {
            return Interlocked.Decrement(ref value);
        }

        public long Get() {
            return value;
        }

        public long GetAndClear() {
            return Interlocked.CompareExchange(ref value, 0L, 0L);
        }

        public long Set(long newValue) {
            return Interlocked.Exchange(ref value, newValue);
        }
    }

    public class AtomicBool(bool initialValue = false) {
        private int value = initialValue ? 1 : 0;
        public bool Value => Get();

        public bool Set(bool newValue) {
            return Interlocked.Exchange(ref value, newValue ? 1 : 0) == 1;
        }

        public bool Set() {
            return Interlocked.Exchange(ref value, 1) == 1;
        }

        public bool Clear() {
            return Interlocked.Exchange(ref value, 0) == 1;
        }

        public bool Toggle() {
            int original, newValue;
            do
            {
                original = value;
                newValue = original == 1 ? 0 : 1;
            } while (Interlocked.CompareExchange(ref value, newValue, original) != original);
            return newValue == 1;
        }

        public bool Get() {
            return value == 1;
        }
    }
} // end namespace

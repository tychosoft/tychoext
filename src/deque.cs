// Copyright (C) 2024 Tycho Softworks.
// This code is licensed under MIT license.

using System;
using System.Collections.Generic;

namespace Tychosoft.Extensions {
    public class Deque<T> {
        private readonly LinkedList<T> deque = new LinkedList<T>();
        public int Count => deque.Count;

        public void PutFront(T item) {
            deque.AddFirst(item);
        }

        public void PutBack(T item) {
            deque.AddLast(item);
        }

        public T GetFront() {
            if (deque.Count == 0 || deque.First == null) throw new InvalidOperationException("Deque is empty.");
            T value = deque.First.Value;
            deque.RemoveFirst();
            return value;
        }

        public T GetBack() {
            if (deque.Count == 0 || deque.Last == null) throw new InvalidOperationException("Deque is empty.");
            T value = deque.Last.Value;
            deque.RemoveLast();
            return value;
        }

        public T Front() {
            if (deque.Count == 0 || deque.First == null) throw new InvalidOperationException("Deque is empty.");
            return deque.First.Value;
        }

        public T Back() {
            if (deque.Count == 0 || deque.Last == null) throw new InvalidOperationException("Deque is empty.");
            return deque.Last.Value;
        }

        public bool IsEmpty() {
            return deque.Count == 0;
        }

        public void Clear() {
            deque.Clear();
        }
    }
}

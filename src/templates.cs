// Copyright (C) 2025 Tycho Softworks.
// This code is licensed under MIT license.

namespace Tychosoft.Extensions {
public static class Templates {
    public static bool InRange<T>(this T value, T min, T max) where T : IComparable<T> {
        return value.CompareTo(min) >= 0 && value.CompareTo(max) <= 0;
    }

    public static T Clamp<T>(this T value, T min, T max) where T : IComparable<T> {
        if (value.CompareTo(min) < 0) return min;
        if (value.CompareTo(max) > 0) return max;
        return value;
    }
}
} // end namespace

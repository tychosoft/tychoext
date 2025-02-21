// Copyright (C) 2024 Tycho Softworks.
// This code is licensed under MIT license.

namespace Tychosoft.Extensions {
public class ScanFile(string path) {
    private readonly string filePath = path;

    public uint Scan(Func<string, bool> scanner) {
        var count = 0u;
        if (!File.Exists(filePath)) throw new FileNotFoundException($"File not found: {filePath}");
        using var reader = new StreamReader(filePath);
        string? line;
        while ((line = reader.ReadLine()) != null) {
            if(!scanner(line)) break;
            count++;
        }
        return count;
    }
}

public class ScanDir(string path) {
    private readonly string dirPath = path;

    public uint Scan(Func<String, bool> scanner) {
        var count = 0u;
        if (!Directory.Exists(dirPath)) throw new DirectoryNotFoundException($"Directory not found: {dirPath}");
        foreach (var entry in Directory.EnumerateFileSystemEntries(dirPath)) {
            if(!scanner(entry)) break;
            count++;
        }
        return count;
    }
}
}

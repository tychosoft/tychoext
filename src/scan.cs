// Copyright (C) 2024 Tycho Softworks.
// This code is licensed under MIT license.

namespace Tychosoft.Extensions {
	public class ScanFile(string path) {
		private readonly string filePath = path;

		public void Scan(Action<string> scanner) {
			if (!File.Exists(filePath)) {
				throw new FileNotFoundException($"File not found: {filePath}");
			}

            using var reader = new StreamReader(filePath);
            string? line;
            while ((line = reader.ReadLine()) != null) {
                scanner(line);
            }
        }
	}

    public class ScanDir(string path) {
        private readonly string dirPath = path;

        public void Scan(Action<string> scanner) {
            if (!Directory.Exists(dirPath)) {
                throw new DirectoryNotFoundException($"Directory not found: {dirPath}");
            }

            foreach (var entry in Directory.EnumerateFileSystemEntries(dirPath)) {
                scanner(entry);
            }
        }
    }
}

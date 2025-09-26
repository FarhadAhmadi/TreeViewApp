using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace TreeViewApp
{
    class Program
    {
        // لیست پوشه‌هایی که نباید نمایش داده بشن
        static readonly HashSet<string> IgnoredFolders = new(StringComparer.OrdinalIgnoreCase)
        {
            "bin", "obj", ".git", ".docker", ".vs", "node_modules" , ".containers"
        };

        // لیست پسوند فایل‌هایی که نباید نمایش داده بشن
        static readonly HashSet<string> IgnoredFileExtensions = new(StringComparer.OrdinalIgnoreCase)
        {
            ".dll", ".exe", ".log", ".tmp"
        };

        static void Main()
        {
            Console.Write("Enter a directory path: ");
            string path = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(path) || !Directory.Exists(path))
            {
                Console.WriteLine("Invalid path.");
                return;
            }

            Console.WriteLine(Path.GetFileName(path) == string.Empty ? path : Path.GetFileName(path));
            PrintDirectory(path, "", true);
        }

        static void PrintDirectory(string dir, string indent, bool isLast)
        {
            string dirName = Path.GetFileName(dir);

            // چک کنیم پوشه توی لیست بلاک نباشه
            if (IgnoredFolders.Contains(dirName))
                return;

            string[] subDirs;
            string[] files;

            try
            {
                subDirs = Directory.GetDirectories(dir)
                                   .OrderBy(d => d, StringComparer.OrdinalIgnoreCase)
                                   .ToArray();

                files = Directory.GetFiles(dir)
                                 .Where(f => !IgnoredFileExtensions.Contains(Path.GetExtension(f)))
                                 .OrderBy(f => f, StringComparer.OrdinalIgnoreCase)
                                 .ToArray();
            }
            catch (UnauthorizedAccessException)
            {
                Console.WriteLine($"{indent}└── [Access Denied]");
                return;
            }

            // چاپ زیرپوشه‌ها
            for (int i = 0; i < subDirs.Length; i++)
            {
                bool lastDir = (i == subDirs.Length - 1) && files.Length == 0;
                Console.WriteLine($"{indent}{(isLast ? "    " : "│   ")}{(lastDir ? "└── " : "├── ")}{Path.GetFileName(subDirs[i])}");
                PrintDirectory(subDirs[i], indent + (isLast ? "    " : "│   "), lastDir);
            }

            // چاپ فایل‌ها
            for (int i = 0; i < files.Length; i++)
            {
                bool lastFile = i == files.Length - 1;
                Console.WriteLine($"{indent}{(isLast ? "    " : "│   ")}{(lastFile ? "└── " : "├── ")}{Path.GetFileName(files[i])}");
            }
        }
    }
}
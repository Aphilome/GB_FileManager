using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FileManager
{
    public class TreeViewer
    {
        // достать список строк, представляющий собой дерево указанного каталога
        public string[] GetDirectoryTree(string directory)
        {
            var tree = new List<string>();
            if (!Directory.Exists(directory))
                tree.Add("|--- Directory not found ---");
            else
                GetTree(directory, tree);
            return tree.ToArray();
        }
        
        // достать файлы и папки в указанной попке 
        private (List<string> files, List<string> directories) GetFilesAndDirectories(string directory, List<string> tree, string tabs)
        {
            string[] entries;
            try
            {
                entries = Directory.GetFileSystemEntries(directory,
                    "*", SearchOption.TopDirectoryOnly);
            }
            catch (Exception)
            {
                tree.Add($"{tabs}|--- Permission inside denied ---");
                return (new List<string>(), new List<string>());
            }
            List<string> directories = entries
                .Where(Directory.Exists)
                .ToList();
            List<string> files = entries
                .Where(i => !directories.Contains(i))
                .ToList();
            directories.Sort();
            files.Sort();
            return (files, directories);
        }
        
        // построить дерево и вернуть списком
        private void GetTree(string directory, List<string> tree)
        {
            tree.Add($"|-[D] {new DirectoryInfo(directory).Name}");
            var entries = GetFilesAndDirectories(directory, tree, "\t");
            foreach (var i in entries.directories)
            {
                tree.Add($"\t|-[D] {new DirectoryInfo(i).Name}");
                var subEntries = GetFilesAndDirectories(i, tree, "\t\t");
                foreach (var j in subEntries.directories)
                    tree.Add($"\t\t|-[D] {new DirectoryInfo(j).Name}");
                foreach (var j in subEntries.files)
                    tree.Add($"\t\t|-[F] {Path.GetFileName(j)}");
            }
            foreach (var i in entries.files)
                tree.Add($"\t|-[F] {Path.GetFileName(i)}");
        }
    }
}
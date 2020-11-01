using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace GenLibrary
{
    public class GenFS : IGenFS
    {
        public void CreateDirectory(string path)
        {
            Directory.CreateDirectory(path);
        }

        public void CreateFile(string path, string contents = null)
        {
            if (contents == null)
            {
                File.Create(path);
            } 
            else 
            {
                File.WriteAllLines(path, new string[] { contents });
            }
        }

        public void DeleteDirectory(string path)
        {
            Directory.Delete(path);
        }

        public bool DirectoryExists(string path)
        {
            return Directory.Exists(path);
        }

        public bool DirectoryIsEmpty(string path)
        {
            return !Directory.EnumerateFileSystemEntries(path).Any();
        }

        public bool FileExists(string path)
        {
            return File.Exists(path);
        }

        public List<string> GetDirectories(string path)
        {
            return Directory.GetDirectories(path).Select(d => Path.GetFileName(d)).ToList();
        }

        public List<string> GetFiles(string path)
        {
            return Directory.GetFiles(path).Select(f => Path.GetFileName(f)).ToList();
        }

        public string ReadFile(string path)
        {
            return File.ReadAllText(path);
        }
    }
}
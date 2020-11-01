using System;
using System.Collections.Generic;
using GenLibrary;

namespace GenTest.Mocks
{
    class MockGenFS : IGenFS
    {
        private DirectoryItem root = new DirectoryItem();

        public bool DirectoryExists(string path)
        {
            return this.root.Exists(path.TrimEnd('/'));
        }

        public void CreateDirectory(string path)
        {
            this.root.Create(path, null, true);
        }

        public void DeleteDirectory(string path)
        {
            this.root.Delete(path.TrimEnd('/'));
        }

        public void CreateFile(string path, string contents = null)
        {
            this.root.Create(path, contents, false);
        }

        public bool FileExists(string path)
        {
            return this.root.Exists(path.TrimEnd('/'));
        }

        public bool DirectoryIsEmpty(string path)
        {
            if (!this.DirectoryExists(path.TrimEnd('/')))
            {
                throw new Exception("Directory does not exist");
            }

            return this.root.IsEmpty(path.TrimEnd('/'));
        }

        public List<string> GetFiles(string path)
        {
            return this.root.GetFiles(path.TrimEnd('/'));
        }

        public List<string> GetDirectories(string path)
        {
            return this.root.GetDirectories(path.TrimEnd('/'));
        }

        public string ReadFile(string path)
        {
            return this.root.Read(path.TrimEnd('/'));
        }
    }
}
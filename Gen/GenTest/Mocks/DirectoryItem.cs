using System.Collections.Generic;
using System.Linq;

namespace GenTest.Mocks
{
    public class DirectoryItem
    {
        public string Name { get; set; }

        public bool IsDirectory { get; set; }

        public string Contents { get; set; }

        public bool IsFile
        {
            get
            {
                return !this.IsDirectory;
            }

            set
            {
                this.IsDirectory = value;
            }
        }

        public List<DirectoryItem> Children { get; set; } = new List<DirectoryItem>();

        public bool Exists(string path)
        {
            var components = path.Split(new[] { '/', '\\' }, 2);

            if (components.Length == 1)
            {
                return this.Children.FirstOrDefault(c => c.Name == path) != null;
            }
            else
            {
                var firstDirectory = components[0];
                var restOfPath = components[1];

                if (firstDirectory == ".")
                {
                    return this.Exists(restOfPath);
                }
                else
                {
                    var existingChild = this.Children.FirstOrDefault(c => c.Name == firstDirectory);
                    if (existingChild == null)
                    {
                        return false;
                    }

                    return existingChild.Exists(restOfPath);
                }
            }
        }

        public void Create(string path, string contents, bool isDirectory)
        {
            var components = path.Split(new[] { '/', '\\' }, 2);

            if (components.Length == 1)
            {
                this.Children.Add(new DirectoryItem
                {
                    Name = path,
                    Contents = contents,
                    IsDirectory = isDirectory
                });
            }
            else
            {
                var firstDirectory = components[0];
                var restOfPath = components[1];

                if (firstDirectory == ".")
                {
                    this.Create(restOfPath, contents, isDirectory);
                }
                else
                {
                    var existingFolder = this.Children.FirstOrDefault(c => c.Name == firstDirectory);
                    if (existingFolder == null)
                    {
                        this.Children.Add(new DirectoryItem
                        {
                            Name = firstDirectory,
                            Contents = contents,
                            IsDirectory = true
                        });

                        existingFolder = this.Children.FirstOrDefault(c => c.Name == firstDirectory);
                    }

                    existingFolder.Create(restOfPath, contents, isDirectory);
                }
            }
        }

        public void Delete(string path)
        {
            var components = path.Split(new[] { '/', '\\' }, 2);

            if (components.Length == 1)
            {
                this.Children.RemoveAll(c => c.Name == path);
            }
            else
            {
                var firstDirectory = components[0];
                var restOfPath = components[1];

                var existingChild = this.Children.FirstOrDefault(c => c.Name == firstDirectory);
                if (existingChild == null)
                {
                    this.Children.Add(new DirectoryItem { Name = firstDirectory });
                    existingChild = this.Children.FirstOrDefault(c => c.Name == firstDirectory);
                }

                if (existingChild != null)
                {
                    existingChild.Delete(restOfPath);
                }
            }
        }

        public bool IsEmpty(string path)
        {
            var components = path.Split(new[] { '/', '\\' }, 2);

            if (components.Length == 1)
            {
                var child = this.Children.FirstOrDefault(c => c.Name == path);
                if (child != null)
                {
                    return child.Children.Count == 0;
                }
            }
            else
            {
                var firstDirectory = components[0];
                var restOfPath = components[1];

                var existingChild = this.Children.FirstOrDefault(c => c.Name == firstDirectory);
                if (existingChild != null)
                {
                    return existingChild.IsEmpty(restOfPath);
                }
            }

            return true;
        }

        public List<string> GetFiles(string path)
        {
            var components = path.Split(new[] { '/', '\\' }, 2);

            if (components.Length == 1)
            {
                var child = this.Children.FirstOrDefault(c => c.Name == path);
                return child.Children.Where(c => !c.IsDirectory).Select(c => c.Name).ToList();
            }
            else
            {
                var firstDirectory = components[0];
                var restOfPath = components[1];

                if (firstDirectory == ".")
                {
                    return this.GetFiles(restOfPath);
                }
                else
                {
                    var existingChild = this.Children.FirstOrDefault(c => c.Name == firstDirectory);
                    return existingChild.GetFiles(restOfPath);
                }
            }
        }

        public List<string> GetDirectories(string path)
        {
            var components = path.Split(new[] { '/', '\\' }, 2);

            if (components.Length == 1)
            {
                var child = this.Children.FirstOrDefault(c => c.Name == path);
                return child.Children.Where(c => c.IsDirectory).Select(c => c.Name).ToList();
            }
            else
            {
                var firstDirectory = components[0];
                var restOfPath = components[1];

                if (firstDirectory == ".")
                {
                    return this.GetDirectories(restOfPath);
                }
                else
                {
                    var existingChild = this.Children.FirstOrDefault(c => c.Name == firstDirectory);
                    return existingChild.GetDirectories(restOfPath);
                }
            }
        }

        public string Read(string path)
        {
            var components = path.Split(new[] { '/', '\\' }, 2);

            if (components.Length == 1)
            {
                return this.Children.First(c => c.Name == path).Contents;
            }
            else
            {
                var firstDirectory = components[0];
                var restOfPath = components[1];

                if (firstDirectory == ".")
                {
                    return this.Read(restOfPath);
                }
                else
                {
                    var existingChild = this.Children.FirstOrDefault(c => c.Name == firstDirectory);
                    return existingChild.Read(restOfPath);
                }
            }
        }
    }
}
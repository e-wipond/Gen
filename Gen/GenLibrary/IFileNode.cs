using System.Collections.Generic;

namespace GenLibrary
{
    public interface IFileNode
    {
        public string FullName { get; set; }

        public string Name { get; }

        public string Contents { get; set; }

        public Dictionary<string, string> Sections {get; set;}

        public bool UsesTemplate { get; }

        public string TemplateName { get; set; }

        public bool IsTemplate { get; }

        public bool IsPartial { get; }

        public string Title { get; set; }

        public IFileNode Parent { get; set; }

        public int FileDepth { get; set; }

        public string Extension { get; }

        public bool IsDraft { get; }

        public int GetTreeDepth();

        public IList<IFileNode> GetSiblings();

        public IList<IFileNode> Children { get; set; }

        public string RelativePath { get; set; }

        public string Next { get; }

        public string Prev { get; }

        public Dictionary<string, string> Keys { get; }
    }
}
using System.Text.RegularExpressions;
using System.IO;
using System.Collections.Generic;

namespace GenLibrary
{
    public class FileNode : IFileNode
    {
        private string contents;

        public string FullName { get; set; }

        public string Name { get { return Path.GetFileNameWithoutExtension(this.FullName); } }

        public bool IsTemplate { get { return this.Frontmatter?.Type == ContentType.Template; } }

        public bool IsPartial { get { return this.Frontmatter?.Type == ContentType.Partial; } }

        public bool UsesTemplate { get { return this.Frontmatter.UsesTemplate; } }

        public bool IsDraft { get { return this.Frontmatter.IsDraft; } }

        public int FileDepth { get; set; }

        public IFileNode Parent { get; set; }

        public string RelativePath { get; set; }

        public string Next { get { return this.Frontmatter.Next; } }

        public string Prev { get { return this.Frontmatter.Prev; } }

        public string Title
        {
            get
            {
                return this.Frontmatter.Title;
            }

            set
            {
                this.Frontmatter.Title = value;
            }

        }

        public string TemplateName
        {
            get
            {
                return this.Frontmatter.Template;
            }
            set
            {
                this.Frontmatter.Template = value;
            }
        }

        public string Contents
        {
            get
            {
                return contents ?? string.Empty;
            }
            set
            {
                contents = value;

                if (this.Frontmatter == null)
                {
                    this.ParseContents();
                }
            }
        }

        public Dictionary<string, string> Sections {get; set;} = new Dictionary<string, string>();

        public int GetTreeDepth()
        {
            var depth = 0;
            IFileNode currentNode = this;

            if (currentNode.Parent == null)
            {
                return depth;
            }

            while (currentNode.Parent != null)
            {
                currentNode = currentNode.Parent;
                depth++;
            }

            return depth;
        }

        public IList<IFileNode> GetSiblings()
        {
            return this.Parent.Children;
        }

        public string Extension
        {
            get
            {
                return Path.GetExtension(this.FullName);
            }
        }

        public Frontmatter Frontmatter { get; set; }
        public IList<IFileNode> Children { get; set; } = new List<IFileNode>();

        public Dictionary<string, string> Keys
        {
            get
            {
                return this.Frontmatter.Keys;
            }
        }

        private void ParseContents()
        {
            if (this.Contents != null)
            {
                var commentRegex = new Regex(@"^<!--[\s\S]*-->\s*", RegexOptions.Compiled);
                var matches = commentRegex.Matches(this.Contents);
                var frontmatterString = string.Empty;

                if (matches.Count == 1)
                {
                    var match = matches[0];
                    frontmatterString = match.Value;

                    this.Contents = this.Contents.Replace(frontmatterString, string.Empty);
                }

                this.Frontmatter = new Frontmatter(frontmatterString);

                var sectionRegex = new Regex(@"#(?<name>\w+){{(?<contents>.*?)}}", RegexOptions.Singleline);
                var sections = sectionRegex.Matches(this.Contents);

                foreach (Match m in sections)
                {
                    var sectionName = m.Groups["name"].Value;
                    var sectionContents = m.Groups["contents"].Value.Trim();

                    this.Sections.Add(sectionName, sectionContents);
                }

                this.Contents = sectionRegex.Replace(this.Contents, string.Empty);
            }
            else
            {
                this.Frontmatter = new Frontmatter(string.Empty);
            }
        }
    }
}
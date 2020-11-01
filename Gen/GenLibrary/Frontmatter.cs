using System.Text.RegularExpressions;
using System.Collections.Generic;
using System;

namespace GenLibrary
{
    public class Frontmatter
    {
        public ContentType Type { get; set; } = ContentType.Content;

        public bool UsesTemplate { get { return this.Template != string.Empty; } }

        public string Template { get; set; } = string.Empty;

        public string Title { get; set; } = string.Empty;

        public bool IsDraft { get; set; }

        public string Next { get; set; }

        public string Prev { get; set; }

        public Dictionary<string, string> Keys { get; set; } = new Dictionary<string, string>();

        public Frontmatter(string rawComment)
        {
            var keyValueRegex = new Regex(@"(?<k>\w+):(?<v>\s*(\w+|"".*""))");
            var matches = keyValueRegex.Matches(rawComment);

            foreach (Match match in matches)
            {
                var key = match.Groups["k"].Value.Trim();
                var val = match.Groups["v"].Value.Replace("\"", string.Empty).Trim();

                this.Keys.Add(key, val);

                if (key == "type")
                {
                    if (val == "template")
                    {
                        this.Type = ContentType.Template;
                    }
                    else if (val == "content")
                    {
                        this.Type = ContentType.Content;
                    }
                    else if (val == "partial")
                    {
                        this.Type = ContentType.Partial;
                    }
                }
                else if (key == "template")
                {
                    this.Template = val;
                }
                else if (key == "title")
                {
                    this.Title = val;
                }
                else if (key == "draft")
                {
                    this.IsDraft = val == "true";
                }
                else if (key == "next")
                {
                    this.Next = val;
                }
                else if (key == "prev")
                {
                    this.Prev = val;
                }
            }
        }
    }
}
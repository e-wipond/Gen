using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace GenLibrary.Transformers
{
    public class PostsIndexTransformer : IFileTransformer
    {
        private IList<IFileNode> pages;

        public PostsIndexTransformer(IList<IFileNode> pages)
        {
            this.pages = pages;
        }

        public IFileNode Transform(IFileNode file)
        {
            if (file.IsTemplate || file.IsPartial)
            {
                return file;
            }

            var tagRegex = new Regex(@"{{posts(\s""(?<path>[^""]*)"")?}}");
            var matches = tagRegex.Matches(file.Contents);

            foreach (Match match in matches)
            {
                var specifiedTitle = match.Groups["path"]?.Value;

                var parent = string.IsNullOrWhiteSpace(specifiedTitle)
                    ? file 
                    : this.pages.Where(p => p.Title == specifiedTitle).FirstOrDefault();

                var posts = parent.Children.OrderBy(f => f.FullName).Reverse().ToList();
                var result = string.Empty;

                foreach(var post in posts)
                {
                    if (post.IsDraft)
                    {
                        continue;
                    }

                    var path = parent == file 
                        ? post.Name + ".html" 
                        : parent.RelativePath + "/" + post.Name + ".html";

                    var date = post.Keys["date"];
                    result += $"- <small>[{date}]</small> [{post.Title}]({path}){Environment.NewLine}";
                }

                file.Contents = tagRegex.Replace(file.Contents, result);
            }

            return file;
        }
    }
}
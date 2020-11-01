using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace GenLibrary.Transformers
{
    public class RenderPartialTransformer : IFileTransformer
    {
        private IList<IFileNode> partials;

        public RenderPartialTransformer(IList<IFileNode> partials)
        {
            this.partials = partials;
        }

        public IFileNode Transform(IFileNode file)
        {
            if (file.IsTemplate || file.IsPartial)
            {
                return file;
            }

            var pattern = @"{{render (?<title>.*)}}";

            var matches = Regex.Matches(file.Contents, pattern);

            if (matches.Count == 0)
            {
                return file;
            }

            foreach (Match match in matches)
            {
                var partialTitle = match.Groups["title"].Value;

                if (this.partials.Any(p => p.Title == partialTitle))
                {
                    var partial = this.partials.Where(p => p.Title == partialTitle).First();
                    file.Contents = file.Contents.Replace(
                        match.Value,
                        partial.Contents
                    );
                }
            }

            return file;
        }
    }
}
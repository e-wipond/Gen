using System.Text.RegularExpressions;

namespace GenLibrary.Transformers
{
    public class CodeTagTransformer : IFileTransformer
    {
        public IFileNode Transform(IFileNode file)
        {
            if (file.IsTemplate || file.IsPartial)
            {
                return file;
            }

            var pattern = "<pre><code>\\s*";

            var matches = Regex.Matches(file.Contents, pattern);

            if (matches.Count == 0)
            {
                return file;
            }

            foreach (Match match in matches)
            {
                var trimmed = match.Value.Trim();
                file.Contents = file.Contents.Replace(match.Value, trimmed);
            }

            return file;
        }
    }
}
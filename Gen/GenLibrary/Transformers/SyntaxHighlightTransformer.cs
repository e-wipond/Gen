using System.Text.RegularExpressions;

namespace GenLibrary.Transformers
{
    public class SyntaxHighlightTransformer : IFileTransformer
    {
        public IFileNode Transform(IFileNode file)
        {
            if (file.IsTemplate || file.IsPartial)
            {
                return file;
            }

            var codeBlock = "<pre><code class=\"scheme(-run)?\">(?<contents>.*?)</code></pre>";
            var matches = Regex.Matches(file.Contents, codeBlock, RegexOptions.Singleline);

            foreach (Match match in matches)
            {
                var contents = match.Groups["contents"].Value;
                var newContents = contents;

                var functions = new string[] {
                    @"(?<pre>\(define\s+\()(?<fn>[^\s\)]+)(?<post>\s+)" // function define
                };

                foreach (var fn in functions)
                {
                    newContents = Regex.Replace(
                        newContents,
                        fn,
                        "${pre}<span class=\"fn\">${fn}</span>${post}");
                }

                var keywords = new string[] {
                    @"(?<pre>\()(?<kw>define)(?<post>\s)", // define
                    @"(?<pre>\()(?<kw>\+)(?<post>\s)", // +
                    @"(?<pre>\()(?<kw>-)(?<post>\s)", // -
                    @"(?<pre>\()(?<kw>\*)(?<post>\s)", // * 
                    @"(?<pre>\()(?<kw>/)(?<post>\s)", // / 
                    @"(?<pre>\()(?<kw>cond)(?<post>\s)", // cond
                    @"(?<pre>\()(?<kw>if)(?<post>\s)", // if
                    @"(?<pre>\()(?<kw>else)(?<post>\s)", // else
                    @"(?<pre>\()(?<kw>and)(?<post>\s)", // and
                    @"(?<pre>\()(?<kw>or)(?<post>\s)", // or
                    @"(?<pre>\()(?<kw>=)(?<post>\s)", // =
                    @"(?<pre>\()(?<kw>&lt;)(?<post>\s)", // >
                    @"(?<pre>\()(?<kw>&gt;)(?<post>\s)", // <
                    @"(?<pre>\()(?<kw>&lt;=)(?<post>\s)", // >=
                    @"(?<pre>\()(?<kw>&gt;=)(?<post>\s)", // <=
                    @"(?<pre>\()(?<kw>lambda)(?<post>\s)", // <=
                };

                foreach (var keyword in keywords)
                {
                    newContents = Regex.Replace(
                        newContents, 
                        keyword, 
                        "${pre}<span class=\"keyword\">${kw}</span>${post}");
                }

                var numbers = new string[] {
                    @"(?<pre>\b)(?<nm>\d+\.?\d*)(?<post>\b)"
                };

                foreach (var number in numbers)
                {
                    newContents = Regex.Replace(
                        newContents, 
                        number, 
                        "${pre}<span class=\"nm\">${nm}</span>${post}");
                }

                var brackets = @"(?<brace>[\(\)])";
                newContents = Regex.Replace(newContents, brackets, "<span class=\"brace\">${brace}</span>");

                file.Contents = file.Contents.Replace(contents, newContents);
            }

            return file;
        }
    }
}
using System;
using System.IO;
using System.Text.RegularExpressions;

namespace GenLibrary.Transformers
{
    public class InlineFileTransformer : IFileTransformer
    {
        private const string pattern = @"{{inline (?<filename>.*)}}";
        private readonly IGenFS fileIO;
        private readonly string sourceDirectory;

        public InlineFileTransformer(IGenFS fileIO, string sourceDirectory)
        {
            this.fileIO = fileIO;
            this.sourceDirectory = sourceDirectory;
        }

        public IFileNode Transform(IFileNode file)
        {
            var matches = Regex.Matches(file.Contents, pattern);

            if (matches.Count == 0)
            {
                return file;
            }

            Console.WriteLine($"Matches: {matches.Count}");

            foreach (Match match in matches)
            {
                var filename = match.Groups["filename"].Value;
                var filepath = Path.Combine(sourceDirectory, file.RelativePath, filename);

                Console.WriteLine(filepath);

                if (fileIO.FileExists(filepath))
                {
                    file.Contents = file.Contents.Replace(
                        match.Value,
                        fileIO.ReadFile(filepath)
                    );
                }
            }

            return file;
        }
    }
}
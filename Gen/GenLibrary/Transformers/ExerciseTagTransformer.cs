using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace GenLibrary.Transformers
{
    public class ExerciseTagTransformer : IFileTransformer
    {
        private Dictionary<int, Dictionary<int, Statistic>> statistics;

        public ExerciseTagTransformer(Dictionary<int, Dictionary<int, Statistic>> stats = null)
        {
            if (stats == null)
            {
                throw new ArgumentNullException(nameof(stats));
            }

            this.statistics = stats;
        }

        public IFileNode Transform(IFileNode file)
        {
            if (file.IsTemplate || file.IsPartial)
            {
                return file;
            }

            var pattern = @"<h3 id=""\w+ \d+\.\d+"" class=""(?<tag>\w+-\w+)"">Exercise (?<title>\d+\.\d+)</h3>";

            var matches = Regex.Matches(file.Contents, pattern);

            if (matches.Count == 0)
            {
                return file;
            }

            foreach (Match match in matches)
            {
                var title = match.Groups["title"].Value;
                var chapter = int.Parse(title.Split('.').First());
                var problem = int.Parse(title.Split('.').Last());

                if (!this.statistics.ContainsKey(chapter))
                {
                    this.statistics.Add(chapter, new Dictionary<int, Statistic>());
                }

                var tag = match.Groups["tag"].Value;

                if (tag == "completed-true")
                {
                    this.statistics[chapter].Add(problem, new Statistic
                    {
                        Completed = true,
                        Link = file.FullName
                    });
                }
                else
                {
                    this.statistics[chapter].Add(problem, new Statistic
                    {
                        Completed = false,
                        Link = file.FullName
                    });
                }
            }

            return file;
        }
    }
}
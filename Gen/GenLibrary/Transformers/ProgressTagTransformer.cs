using System;
using System.Collections.Generic;

namespace GenLibrary.Transformers
{
    public class ProgressTagTransformer : IFileTransformer
    {
        private Dictionary<int, Dictionary<int, Statistic>> statistics;

        public ProgressTagTransformer(Dictionary<int, Dictionary<int, Statistic>> stats)
        {
            if (stats is null)
            {
                throw new ArgumentNullException(nameof(stats));
            }

            this.statistics = stats;
        }

        public IFileNode Transform(IFileNode file)
        {
            if (file.IsTemplate)
            {
                return file;
            }

            if (!file.Contents.Contains("{{progress}}"))
            {
                return file;
            }

            var numExercises = new Dictionary<int, int>();

            numExercises.Add(1, 46);
            numExercises.Add(2, 97);
            numExercises.Add(3, 82);
            numExercises.Add(4, 79);
            numExercises.Add(5, 52);

            var divs = string.Empty;

            foreach (KeyValuePair<int, int> kvp in numExercises)
            {
                for (var i = 1; i <= kvp.Value; i++)
                {
                    var style = "link";
                    var link = string.Empty;

                    if (this.statistics.ContainsKey(kvp.Key) && this.statistics[kvp.Key].ContainsKey(i))
                    {
                        if (this.statistics[kvp.Key][i].Completed == true)
                        {
                            style += " done";
                        }
                        else
                        {
                            style += " half";
                        }

                        var href = $"{this.statistics[kvp.Key][i].Link}#Exercise {kvp.Key}.{i}";

                        link = $"<a class=\"problem-link\" href=\"{href}\"></a>";
                    }

                    divs += $"<div class=\"{style}\" title=\"Exercise {kvp.Key}.{i}\">{link}</div>\n";
                }
            }

            var contents = $"<div id=\"vis\">{Environment.NewLine}{divs}{Environment.NewLine}</div>";

            file.Contents = file.Contents.Replace("{{progress}}", contents);

            return file;
        }
    }
}
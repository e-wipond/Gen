using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace GenLibrary.Transformers
{
    public class MarkdownTransformer : IFileTransformer
    {
        private Dictionary<string, string> inlineRules = new Dictionary<string, string>
        {
            { @"\*\*(?<content>.*?)\*\*", "<strong>${content}</strong>" },
            { @"\*(?<content>.*?)\*", "<em>${content}</em>" },
            { "`(?<content>[^`]*)`", "<code>${content}</code>" },
            { @"\[(?<text>[^\]]*?)\]\((?<link>.*?)\)", "<a href=\"${link}\">${text}</a>"}
        };

        private Dictionary<string, string> blockRules = new Dictionary<string, string>
        {
            { "^# (?<heading>.*)$", "<h1>${heading}</h1>" },
            { "^## (?<heading>.*)$", "<h2>${heading}</h2>" },
            { "^### (?<heading>.*)$", "<h3>${heading}</h3>" },
            { "^#### (?<heading>.*)$", "<h4>${heading}</h4>" },
            { "^---$", "<hr>" },
            { "^(?<tag>{{.*}})", "${tag}" },
        };

        private Dictionary<string, string> singlelineRules = new Dictionary<string, string>
        {
            { "^(?<html><.*>)$", "${html}" }, 
        };

        public IFileNode Transform(IFileNode file)
        {
            if (file.Extension != ".md")
            {
                return file;
            }

            if (file.IsPartial || file.IsTemplate)
            {
                return file;
            }

            var sections = BuildPageSectionsFromContents(file.Contents);
            var rendered = RenderSections(sections);

            file.Contents = CombineSecions(rendered);
            file.FullName = $"{file.Name}.html";

            return file;
        }

        private string CombineSecions(List<string> rendered)
        {
            var result = string.Empty;
            for (var i = 0; i < rendered.Count; i++)
            {
                var section = rendered[i].Trim();
                if (string.IsNullOrWhiteSpace(section))
                {
                    continue;
                }

                result += section + Environment.NewLine;
                if (i != rendered.Count - 1)
                {
                    result += Environment.NewLine;
                }
            }

            return result;
        }

        private List<string> BuildPageSectionsFromContents(string contents)
        {
            var sections = new List<string>();

            var doubleLineBreak = $"{Environment.NewLine}{Environment.NewLine}";

            var blocks = contents.Split(doubleLineBreak);

            for (var i = 0; i < blocks.Length; i++)
            {
                var block = blocks[i].Trim();

                if (string.IsNullOrWhiteSpace(block))
                {
                    continue;
                } 
                else if (block.StartsWith("```") && !block.EndsWith("```"))
                {
                    var section = block;
                    while (true) 
                    {
                        if (i == blocks.Length - 1)
                        {
                            throw new Exception("Can't find end of code block");
                        }

                        i++;
                        var nextBlock = blocks[i];
                        section += $"{doubleLineBreak}{nextBlock}";

                        if (section.EndsWith("```"))
                        {
                            sections.Add(section);
                            break;
                        }
                    }
                }
                else if (Regex.IsMatch(block, "^<\\w+>") && !Regex.IsMatch(block, "</\\w+>$"))
                {
                    var section = block;

                    while (true)
                    {
                        if (i == blocks.Length - 1)
                        {
                            throw new Exception("Can't find end of html block");
                        }

                        i++;
                        var nextBlock = blocks[i].Trim();
                        section += $"{doubleLineBreak}{nextBlock}";

                        if (Regex.IsMatch(section, "</\\w+>$"))
                        {
                            sections.Add(section);
                            break;
                        }
                    }
                }
                else 
                {
                    sections.Add(block);
                }
            }

            return sections;
        }

        private List<string> RenderSections(List<string> sections)
        {
            var result = new List<string>();

            foreach (var section in sections)
            {
                var isRendered = false;

                foreach (var rule in blockRules)
                {
                    if (Regex.IsMatch(section, rule.Key))
                    {
                        result.Add(Regex.Replace(section, rule.Key, rule.Value));
                        isRendered = true;
                        break;
                    }
                }

                if (isRendered) continue;

                foreach (var rule in singlelineRules)
                {
                    if (Regex.IsMatch(section, rule.Key, RegexOptions.Singleline))
                    {
                        result.Add(Regex.Replace(section, rule.Key, rule.Value));
                        isRendered = true;
                        break;
                    }
                }

                if (isRendered) continue;

                if (Regex.IsMatch(section, "^```.*```$", RegexOptions.Singleline))
                {
                    result.Add(RenderCodeBlock(section));
                }
                else if (Regex.IsMatch(section, "^- .*"))
                {
                    result.Add(RenderUnorderedList(section));
                }
                else 
                {
                    var renderedSection = section;

                    foreach (var rule in inlineRules)
                    {
                        renderedSection = Regex.Replace(renderedSection, rule.Key, rule.Value);
                    }

                    var codeSections = Regex.Matches(renderedSection, "<code>(?<c>.*?)</code>");
                    foreach (Match m in codeSections)
                    {
                        var escapedCode = "<code>" + m.Groups["c"].Value.Replace("<", "&lt;").Replace(">", "&gt;") + "</code>";
                        renderedSection = renderedSection.Replace(m.Value, escapedCode);
                    }

                    result.Add($"<p>{renderedSection.Trim()}</p>");
                }
            }

            return result;
        }

        private string RenderUnorderedList(string section)
        {
            var lines = section.Split(Environment.NewLine);
            var rendered = string.Empty;
            foreach (var line in lines)
            {
                rendered += "    " + Regex.Replace(line, "^- (?<li>.*)", "<li>${li}</li>") + Environment.NewLine;
            }

            foreach (var rule in inlineRules)
            {
                rendered = Regex.Replace(rendered, rule.Key, rule.Value);
            }

            var codeSections = Regex.Matches(rendered, "<code>(?<c>.*?)</code>");
            foreach (Match m in codeSections)
            {
                var escapedCode = "<code>" + m.Groups["c"].Value.Replace("<", "&lt;").Replace(">", "&gt;") + "</code>";
                rendered = rendered.Replace(m.Value, escapedCode);
            }

            return $"<ul>{Environment.NewLine}{rendered}</ul>";
        }

        private string RenderCodeBlock(string section)
        {
            var multilineCodePattern = @"```(?<lang>[\S]*)(?<run> [\S]*)?(\n|\r|\r\n)(?<code>[^`]*)```";
            var match = Regex.Match(section, multilineCodePattern, RegexOptions.Singleline);

            var code = match.Groups["code"].Value;
            code = code.Replace("<", "&lt;");
            code = code.Replace(">", "&gt;");
            code = code.Trim();

            var lang = match.Groups["lang"].Value;

            var run = match.Groups["run"]?.Value ?? string.Empty;

            if (!string.IsNullOrEmpty(run)) {
                lang += $"-run";
            }

            return section.Replace(match.Value, $"<pre><code class=\"{lang}\">{code}</code></pre>");
        }
    }
}
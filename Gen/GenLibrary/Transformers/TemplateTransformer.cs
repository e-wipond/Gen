using System.Collections.Generic;
using System.Linq;

namespace GenLibrary.Transformers
{
    public class TemplateTransformer : IFileTransformer
    {
        private readonly IList<IFileNode> templates;

        public TemplateTransformer(IList<IFileNode> templates)
        {
            this.templates = templates;            
        }

        public IFileNode Transform(IFileNode file)
        {
            if (file.UsesTemplate)
            {
                if (this.templates.Any(f => f.Name == file.TemplateName))
                {
                    file.Contents = this.templates
                        .Where(f => f.Name == file.TemplateName)
                        .First()
                        .Contents
                        .Replace("{{content}}", file.Contents.Trim());

                    file.TemplateName = string.Empty;
                }
            }

            return file;
        }
    }
}
using System;
using System.Linq;

namespace GenLibrary.Transformers
{
    public class NextPreviousTransformer : IFileTransformer
    {
        public IFileNode Transform(IFileNode file)
        {
            if (file.IsTemplate || file.IsPartial)
            {
                return file;
            }

            if (string.IsNullOrEmpty(file.Prev) && string.IsNullOrEmpty(file.Next))
            {
                return file;
            }

            var leftLink = string.Empty;
            var rightLink = string.Empty;

            var pages = file.GetSiblings();

            if (!string.IsNullOrEmpty(file.Prev))
            {
                var prevPage = pages.Where(p => p.Title == file.Prev).FirstOrDefault();
                if (prevPage != null)
                {
                    leftLink = $"<a href=\"{prevPage.Name}.html\" title=\"{prevPage.Title}\">Previous</a>";
                }
            }
            else 
            {
                leftLink = "<span>&nbsp;</span>";
            }

            if (!string.IsNullOrEmpty(file.Next))
            {
                var nextPage = pages.Where(p => p.Title == file.Next).FirstOrDefault();
                if (nextPage != null)
                {
                    rightLink = $"<a href=\"{nextPage.Name}.html\" title=\"{nextPage.Title}\">Next</a>";
                }
            }
            else 
            {
                rightLink = "<span>&nbsp;</span>";
            }

            file.Contents += $"{Environment.NewLine}{Environment.NewLine}<div style=\"display: flex; justify-content: space-between\">{leftLink} {rightLink}</div>";

            return file;
        }
    }
}
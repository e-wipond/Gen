using System.Text.RegularExpressions;

namespace GenLibrary.Transformers
{
    public class CleanupTransformer : IFileTransformer
    {
        public IFileNode Transform(IFileNode file)
        {
            if (file.IsTemplate || file.IsPartial)
            {
                return file;
            }

            file.Contents = Regex.Replace(file.Contents, @"{{\w+}}", string.Empty);

            return file;
        }
    }
}
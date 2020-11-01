namespace GenLibrary.Transformers
{
    public class RelativeRootTagTransformer : IFileTransformer
    {
        public IFileNode Transform(IFileNode file)
        {
            if (file.IsTemplate || file.IsPartial)
            {
                return file;
            }

            var currentDepth = file.FileDepth;

            var relativePath = string.Empty;
            if (currentDepth == 0)
            {
                relativePath = "./";
            }
            else
            {
                while (currentDepth > 0)
                {
                    relativePath += "../";
                    currentDepth--;
                }
            }

            file.Contents = file.Contents.Replace("{{relative-root}}", relativePath);

            return file;
        }
    }
}
namespace GenLibrary.Transformers
{
    public class BreadcrumbTagTransformer : IFileTransformer
    {
        public IFileNode Transform(IFileNode file)
        {
            if (file.IsTemplate || file.IsPartial)
            {
                return file;
            }

            var treeDepth = file.GetTreeDepth();

            if (treeDepth != 0)
            {
                var parent = file.Parent;
                var pathToParent = file.FileDepth != treeDepth
                    ? $"./{parent.Name}.html"
                    : $"../{parent.Name}.html";

                var result = $"<a href=\"{pathToParent}\">{parent.Title}</a>";

                var currentPath = file.FileDepth != treeDepth ? "../" : "../../";
                while (parent.Parent != null)
                {
                    parent = parent.Parent;
                    pathToParent = $"{currentPath}{parent.Name}.html";

                    result = $"<a href=\"{pathToParent}\">{parent.Title}</a> <span>></span> {result}";

                    currentPath = $"../{currentPath}";
                }

                file.Contents = file.Contents.Replace("{{breadcrumbs}}", result);
            }

            return file;
        }
    }
}
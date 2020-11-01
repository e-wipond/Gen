using System.Collections.Generic;

namespace GenLibrary.Transformers
{
    public class KeyTagTransformer : IFileTransformer
    {
        public IFileNode Transform(IFileNode file)
        {
            foreach (KeyValuePair<string, string> kvp in file.Keys)
            {
                file.Contents = file.Contents.Replace($"{{{{{kvp.Key}}}}}", kvp.Value);
            }

            foreach (KeyValuePair<string, string> kvp in file.Sections)
            {
                file.Contents = file.Contents.Replace($"{{{{{kvp.Key}}}}}", kvp.Value);
            }

            return file;
        }
    }
}
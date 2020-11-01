using System.Collections.Generic;

namespace GenLibrary
{
    public interface IGenFS
    {
       bool DirectoryExists(string path);

       void CreateDirectory(string path);

       void DeleteDirectory(string path);

       void CreateFile(string path, string contents = null);

       bool FileExists(string path);

       bool DirectoryIsEmpty(string path);

       List<string> GetFiles(string path);
       
       List<string> GetDirectories(string path);

       string ReadFile(string path);
    }
}
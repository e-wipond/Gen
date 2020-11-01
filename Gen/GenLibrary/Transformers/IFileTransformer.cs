namespace GenLibrary.Transformers
{
    public interface IFileTransformer
    {
        IFileNode Transform(IFileNode file);
    }
}
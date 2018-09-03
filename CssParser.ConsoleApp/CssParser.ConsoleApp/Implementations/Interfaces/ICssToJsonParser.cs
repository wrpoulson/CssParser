
namespace CssParser.ConsoleApp.Implementations.Interfaces
{
    public interface ICssToJsonParser
    {
        void ParseSourceCssFiles(string sourceFilesPath);
        void ParseSourceCssFile(string sourceFilePath, string currentFileName);
    }
}


namespace CssParser.ConsoleApp.Implementations.Interfaces
{
    public interface IJsonToSqlParser
    {
        void ParseTransDetJsonFiles(string sourceFilesDirectory);

        void ParseTransDetJsonFile(string sourceFilePath, string currentFileName);
    }
}

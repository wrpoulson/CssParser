using System.IO;
using System.Linq;

namespace CssParser.ConsoleApp.Utilities
{
  public class TransDetailJsonParser
  {
    const string OUTPUT_PATH = @"Content\OutputSQL\";

    public void ParseTransDetJsonFiles(string sourceFilesPath)
    {
      if (!Directory.Exists(sourceFilesPath)) throw new DirectoryNotFoundException();
      FileInfo[] files = new DirectoryInfo(sourceFilesPath).GetFiles("*.json");
      files.ToList().ForEach(file => ParseSourceCssFile(file.FullName, file.Name));
    }

    public void ParseSourceCssFile(string sourceFilePath, string currentFileName)
    {
      //TODO: WRP make the thing
    }
  }
}

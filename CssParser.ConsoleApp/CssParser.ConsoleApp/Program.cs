using CssParser.ConsoleApp.Utilities.Parsers.Css;
using CssParser.ConsoleApp.Utilities.Parsers.Json;
using System.IO;

namespace CssParser.ConsoleApp
{
  class Program
  {
    const string SOURCE_CSS_DIRECTORY = @"Content\SourceCSS";
    const string OUTPUT_JSON_DIRECTORY = @"Content\OutputJSON";
    const string OUTPUT_SQL_DIRECTORY = @"Content\OutputSQL";

    static void Main(string[] args)
    {
      var claimFormCssParser = new CssToJsonParser();
      var transDetailJsonParser = new JsonToSqlParser();
      var includeModifiedCss = true;
      ClearWorkingDirectories();
      claimFormCssParser.ParseSourceCssFiles(SOURCE_CSS_DIRECTORY, includeModifiedCss);
      transDetailJsonParser.ParseTransDetJsonFiles(OUTPUT_JSON_DIRECTORY, includeModifiedCss);
    }

    public static void ClearWorkingDirectories()
    {
      if (Directory.Exists(OUTPUT_JSON_DIRECTORY))
      {
        Directory.Delete(OUTPUT_JSON_DIRECTORY, true);
      }
      if (Directory.Exists(OUTPUT_SQL_DIRECTORY))
      {
        Directory.Delete(OUTPUT_SQL_DIRECTORY, true);
      }
    }
  }
}

using System.IO;
using CssParser.ConsoleApp.Implementations.Interfaces;

// XP7092
namespace CssParser.ConsoleApp.Utilities.Parsers.ClaimForm
{
    public class TransDetailParser : IParser
    {
        const string OUTPUT_JSON_DIRECTORY = @"Content\OutputJSON";
        const string OUTPUT_SQL_DIRECTORY = @"Content\OutputSQL";
        private ICssToJsonParser _claimFormCssParser = new CssToJsonParser();
        private IJsonToSqlParser _transDetailJsonParser = new JsonToSqlParser();

        public void Run(string sourceDirectory)
        {
            ClearWorkingDirectories();
            _claimFormCssParser.ParseSourceCssFiles(sourceDirectory);
            _transDetailJsonParser.ParseTransDetJsonFiles(OUTPUT_JSON_DIRECTORY);
        }

        private static void ClearWorkingDirectories()
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

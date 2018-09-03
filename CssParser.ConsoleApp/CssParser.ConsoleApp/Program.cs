using CssParser.ConsoleApp.Utilities.Parsers.ClaimForm;
using CssParser.ConsoleApp.Implementations.Interfaces;

namespace CssParser.ConsoleApp
{
  class Program
  {
    const string SOURCE_CSS_DIRECTORY = @"Content\SourceCSS";

    static void Main(string[] args)
    {
            IParser parser = new TransDetailParser();
            parser.Run(SOURCE_CSS_DIRECTORY);
    }
  }
}

using CssParser.ConsoleApp.Utilities.Parsers.ClaimForm;
using CssParser.ConsoleApp.Implementations.Interfaces;

namespace CssParser.ConsoleApp
{
  class Program
  {
    static void Main(string[] args)
    {
            IParser parser = new TransDetailParser();
            parser.Run();
    }
  }
}

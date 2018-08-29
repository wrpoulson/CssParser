using CssParser.ConsoleApp.Utilities;

namespace CssParser.ConsoleApp
{
  class Program
  {
    static void Main(string[] args)
    {
      var ClaimFormCSSParser = new ClaimFormCSSParser();
      ClaimFormCSSParser.ParseSourceCSSFile();
    }
  }
}

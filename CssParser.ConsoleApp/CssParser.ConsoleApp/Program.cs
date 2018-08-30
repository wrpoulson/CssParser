using CssParser.ConsoleApp.Utilities;

namespace CssParser.ConsoleApp
{
  class Program
  {
    static void Main(string[] args)
    {
      var claimFormCssParser = new ClaimFormCssParser();
      var transDetailJsonParser = new TransDetailJsonParser();
      claimFormCssParser.ParseSourceCssFiles(@"Content\SourceCSS");
      transDetailJsonParser.ParseTransDetJsonFiles(@"Content\OutputJSON");
    }
  }
}

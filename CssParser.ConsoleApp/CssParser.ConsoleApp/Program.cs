using CssParser.ConsoleApp.Utilities.ClaimForm;

namespace CssParser.ConsoleApp
{
  class Program
  {
    static void Main(string[] args)
    {
      var claimFormCssParser = new CssToJsonParser();
      var transDetailJsonParser = new JsonToSqlParser();
      claimFormCssParser.ParseSourceCssFiles(@"Content\SourceCSS");
      transDetailJsonParser.ParseTransDetJsonFiles(@"Content\OutputJSON", true);
    }
  }
}

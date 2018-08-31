
namespace CssParser.ConsoleApp.Models.ClaimForm
{
  public class TransDetailParseResult
  {
    public int PotentialLinesIdentified { get; set; }
    public int SavedTransDetails { get; set; }
    public TransDetailParseErrorInfo ErrorDetails { get; set; }
  }
}

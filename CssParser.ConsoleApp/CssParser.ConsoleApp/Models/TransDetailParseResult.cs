using System.Collections.Generic;

namespace CssParser.ConsoleApp.Models
{
  public class TransDetailParseResult
  {
    public int PotentialLinesIdentified { get; set; }
    public int SavedTransDetails { get; set; }
    public int PotentialLinesEndedWithComma { get; set; }
    public int XpCssWidthNull { get; set; }
    public int XpCssLeftNull { get; set; }
    public int XpCssTopNull { get; set; }
    public int SavedErrorCount => SavedErrors?.Count ?? 0;
    public int UnsavedErrorCount => UnsavedErrors?.Count ?? 0;
    public List<TransDetailError> SavedErrors { get; set; }
    public List<TransDetailError> UnsavedErrors { get; set; }
  }
}

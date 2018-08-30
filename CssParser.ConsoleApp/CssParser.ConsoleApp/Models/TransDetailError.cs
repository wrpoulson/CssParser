
namespace CssParser.ConsoleApp.Models
{
  public class TransDetailError
  {
    public string FieldName { get; set; }
    public int FileLineNumber { get; set; }
    public bool IsTransDetailSaved { get; set; }
    public bool IsErrorDueToHeight20Px { get; set; }
    public bool IsErrorDueToBrace { get; set; }
    public bool IsLineEndingWithComma { get; set; }
    public bool IsXpCssLeftNull { get; set; }
    public bool IsXpCssTopNull { get; set; }
    public bool IsXpCssWidthNull { get; set; }
  }
}

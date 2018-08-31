
namespace CssParser.ConsoleApp.Models.ClaimForm
{
  public class TransDetailError
  {
    public int ErrorRecordId { get; set; }
    public string FieldName { get; set; }
    public int FileLineNumber { get; set; }
    public bool IsErrorDueToHeight20Px { get; set; }
    public bool IsErrorDueToBrace { get; set; }
    public bool IsLineEndingWithComma { get; set; }
    public bool IsXpCssLeftNull { get; set; }
    public bool IsXpCssTopNull { get; set; }
    public bool IsXpCssWidthNull { get; set; }
    public TransDetail InvalidTransDetail { get; set; }
  }
}

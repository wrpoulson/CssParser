
namespace CssParser.ConsoleApp.Models
{
  public class TransDetail
  {
    public int JsonRecordId { get; set; }
    public int FileLineNumber { get; set; }
    public string Type => "HDR";
    public string FieldName { get; set; }
    public int? Xp_Css_Left { get; set; }
    public int? Xp_Css_Top { get; set; }
    public int? Xp_Css_Width { get; set; }
  }
}

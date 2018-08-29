using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CssParser.ConsoleApp.Models
{
  public class TransDetRecord
  {
    public int JsonRecordId { get; set; }
    public int FileReadLineId { get; set; }
    public string Type => "HDR";
    public string FieldName { get; set; }
    public int? Xp_Css_Width { get; set; }
    public int? Xp_Css_Left { get; set; }
    public int? Xp_Css_Top { get; set; }
  }
}

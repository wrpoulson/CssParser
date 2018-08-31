using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CssParser.ConsoleApp.Models.ClaimForm
{
  public class TransDetailParseErrorInfo
  {
    public int TotalLinesEndedWithComma => Errors.Where(m => m.IsLineEndingWithComma).Count();
    public int TotalLinesNotFollowedByBraceLine => Errors.Where(m => m.IsErrorDueToBrace).Count();
    public int TotalLinesNotFollowedBy20PxHeightLine => Errors.Where(m => m.IsErrorDueToHeight20Px).Count();
    public int TotalXpCssLeftNull => ErrorsWithInvalidTransDetails.Where(m => m.InvalidTransDetail.Xp_Css_Left == null).Count();
    public int TotalXpCssTopNull => ErrorsWithInvalidTransDetails.Where(m => m.InvalidTransDetail.Xp_Css_Top == null).Count();
    public int TotalXpCssWidthNull => ErrorsWithInvalidTransDetails.Where(m => m.InvalidTransDetail.Xp_Css_Width == null).Count();
    public int TotalErrors => Errors?.Count ?? 0;
    public int TotalPartialTransDetailsSaved => ErrorsWithInvalidTransDetails.Count();
    public List<TransDetailError> Errors { get; set; }
    public IEnumerable<TransDetailError> ErrorsWithInvalidTransDetails => Errors.Where(m => m.InvalidTransDetail != null);
  }
}

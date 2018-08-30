using System.Collections.Generic;

namespace CssParser.ConsoleApp.Models
{
  public class TransDetailParseResponse
  {
    public List<TransDetail> ParsedTransDetails { get; set; }
    public TransDetailParseResult ParseResult { get; set; }
  }
}

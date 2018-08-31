using System.IO;
using System.Linq;
using System.Collections.Generic;
using CssParser.ConsoleApp.Models.ClaimForm;
using Newtonsoft.Json;
using System;
using CssParser.ConsoleApp.Utilities.SqlQueryBuilders;
using CssParser.ConsoleApp.Utilities.SqlQueryBuilders.ClaimForm;

namespace CssParser.ConsoleApp.Utilities.Parsers.Json
{
  public class JsonToSqlParser
  {
    const string OUTPUT_PATH = @"Content\OutputSQL\";

    public void ParseTransDetJsonFiles(string sourceFilesDirectory, bool includeModifiedCss)
    {
      if (!Directory.Exists(sourceFilesDirectory)) throw new DirectoryNotFoundException();
      FileInfo[] allFiles = new DirectoryInfo(sourceFilesDirectory).GetFiles("*.TransDetails.json");
      ParseAllTransDetJsonFiles(allFiles, includeModifiedCss);
    }

    public void ParseTransDetJsonFile(string sourceFilePath, string currentFileName, bool includeModifiedCss)
    {
      List<TransDetail> transDetails = JsonConvert.DeserializeObject<List<TransDetail>>(File.ReadAllText(sourceFilePath));
      if (includeModifiedCss)
      {
        string modifiedCssFilePath = sourceFilePath.Replace("\\OutputJSON\\", "\\OutputJSON\\Modified\\").Replace(currentFileName, $"{currentFileName}.Modified");
        var transDetailsFromModifiedCss = JsonConvert.DeserializeObject<List<TransDetail>>(File.ReadAllText(modifiedCssFilePath));
        if(transDetailsFromModifiedCss != null && transDetailsFromModifiedCss.Any())
        {
          transDetails.AddRange(transDetailsFromModifiedCss);
        }
      }
      var sqlQuery = BuildSqlQuery(transDetails);
      SaveSqlQueryScript(sqlQuery, currentFileName);
    }

    private void ParseAllTransDetJsonFiles(FileInfo[] allFiles, bool includeModifiedCss)
    {
      allFiles.ToList().ForEach(file => ParseTransDetJsonFile(file.FullName, file.Name.Substring(0, file.Name.IndexOf(".css")), includeModifiedCss));
    }

    private string BuildSqlQuery(List<TransDetail> transDetails)
    {
      var transDetailUpdates = transDetails.Select(t => { return SqbTransDetail.SqlIfElseUpdateTransDet(t); });

      var ifTableExistsUpdateElseRollback = SqlQueryBuilder.IfElse(
                                                SqlQueryBuilder.TableExistsCondition("dbo", "TRANS_DET"),    // condition
                                                $"{string.Join("\n", transDetailUpdates)}\n",                // if
                                                SqbTransDetail.PrintTableDoesNotExistRollback("TRANS_DET")); // else


      return $"{SqlQueryBuilder.Print("\t--SCRIPT START--")}\n\n{SqlQueryBuilder.Use("CODETABLES")}{ifTableExistsUpdateElseRollback}\n{SqlQueryBuilder.Print("\t--SCRIPT END--")}";
    }

    private void SaveSqlQueryScript(string query, string outputFileName)
    {
      if (!Directory.Exists(OUTPUT_PATH))
      {
        Directory.CreateDirectory(OUTPUT_PATH);
      }
      File.WriteAllText($"{OUTPUT_PATH}{outputFileName}.Updates.sql", query);
    }
  }
}

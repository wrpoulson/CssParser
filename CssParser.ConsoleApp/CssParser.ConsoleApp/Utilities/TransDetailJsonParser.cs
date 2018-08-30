using System.IO;
using System.Linq;
using System.Collections.Generic;
using CssParser.ConsoleApp.Models;
using Newtonsoft.Json;
using System;

namespace CssParser.ConsoleApp.Utilities
{
  public class TransDetailJsonParser
  {
    const string OUTPUT_PATH = @"Content\OutputSQL\";

    public void ParseTransDetJsonFiles(string sourceFilesPath, bool parseOnlyMostRecentFiles)
    {
      if (!Directory.Exists(sourceFilesPath)) throw new DirectoryNotFoundException();
      FileInfo[] allFiles = new DirectoryInfo(sourceFilesPath).GetFiles("*.TransDetails.json");
      if (parseOnlyMostRecentFiles)
      {
        ParseMostRecentTransDetJsonFiles(allFiles);
      }
      else
      {
        ParseAllTransDetJsonFiles(allFiles);
      }
    }

    public void ParseTransDetJsonFile(string sourceFilePath, string currentFileName)
    {
      //TODO: WRP make the thing
      List<TransDetail> transDetails = JsonConvert.DeserializeObject<List<TransDetail>>(File.ReadAllText(sourceFilePath));
      var sqlQuery = BuildSqlQuery(transDetails);
      SaveSqlQueryScript(sqlQuery, currentFileName);
    }

    private void ParseMostRecentTransDetJsonFiles(FileInfo[] allFiles)
    {
      var fileNames = allFiles.Select(m => m.Name).ToArray();
      var uniqueFilePrecursors = Array.ConvertAll(fileNames, fileName => fileName.Substring(0, fileName.IndexOf(".css"))).Distinct();
      var mostRecentFiles = uniqueFilePrecursors.Select(m => allFiles.OrderByDescending(f => f.CreationTime).First(f => f.Name.StartsWith(m))).ToList();
      mostRecentFiles.ForEach(file => ParseTransDetJsonFile(file.FullName, file.Name.Substring(0, file.Name.IndexOf(".css"))));
    }

    private void ParseAllTransDetJsonFiles(FileInfo[] allFiles)
    {
      allFiles.ToList().ForEach(file => ParseTransDetJsonFile(file.FullName, file.Name.Substring(0, file.Name.IndexOf(".css"))));
    }

    private void SaveSqlQueryScript(string query, string outputFileName)
    {
      if (!Directory.Exists(OUTPUT_PATH))
      {
        Directory.CreateDirectory(OUTPUT_PATH);
      }
      File.WriteAllText($"{OUTPUT_PATH}{outputFileName}_{DateTime.Now.ToString("yyyy-dd-M--HH-mm-ss")}.Updates.sql", query);
    }

    private string SqlUse(string database) => $"USE {database}\nGO\n\n";

    private string SqlBeginTransaction() => $"BEGIN TRANSACTION;\nGO\n";

    private string SqlRollbackTransaction() => "\nROLLBACK TRANSACTION;";

    private string SqlPrint(string text) => $"\nPRINT CAST(GETDATE() as Datetime2(3))\nPRINT N'{text}'";

    private string SqlIf(string condition, string conditionalOperation) => $"IF {condition}\nBEGIN\n{conditionalOperation.Replace("\n","\n\t")}\nEND;";

    private string SqlIfElse(string condition, string ifConditionalOperation, string elseConditionalOperation)
    {
      return $"\nIF {condition}\nBEGIN{ifConditionalOperation.Replace("\n", "\n\t")}\nEND;\n\nELSE\nBEGIN{elseConditionalOperation.Replace("\n", "\n\t")}\nEND;";
    }

    private string SqlTableExistsCondition(string tableName)
    {
      return $"(EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = '{tableName}'))";
    }

    private string UpdateTransDetRecord(TransDetail transDetail)
    {
      return $"{PrintTransDetailUpdate(transDetail)}\n\nUPDATE TRANS_DET\nSET XP_CSS_LEFT = '{transDetail.Xp_Css_Left}', XP_CSS_TOP = '{transDetail.Xp_Css_Top}', XP_CSS_WIDTH = '{transDetail.Xp_Css_Width}'\nWHERE FIELDNAME = '{transDetail.FieldName}' AND [TYPE] = '{transDetail.Type}' AND [NAME] = '{transDetail.Name}'";
    }

    private string SelectCountForTransDetRecord(TransDetail transDetail)
    {
      return $"(SELECT COUNT(*) FROM TRANS_DET WHERE FIELDNAME = '{transDetail.FieldName}' AND [TYPE] = '{transDetail.Type}' AND [NAME] = '{transDetail.Name}') = 1";
    }

    private string PrintTransDetailUpdateError(TransDetail transDetail)
    {
      return SqlPrint($"\tWARNING: Record was NOT UPDATED. --- FIELDNAME: {transDetail.FieldName} --- NAME: {transDetail.Name} --- TYPE: {transDetail.Type} ");
    }

    private string PrintTransDetailUpdate(TransDetail transDetail)
    {
      return SqlPrint($"\tINFO: Updating record. --- FIELDNAME: {transDetail.FieldName} --- NAME: {transDetail.Name} --- TYPE: {transDetail.Type} ");
    }

    private string PrintTableDoesNotExistRollback(string tableName)
    {
      return $"{SqlPrint($"ERROR: Table: {tableName} does NOT exist. Commencing transaction rollback.")}{SqlRollbackTransaction()}";
    }

    private string SqlIfElseUpdateTransDet(TransDetail transDetail)
    {
      return $"\t{SqlIfElse(SelectCountForTransDetRecord(transDetail), UpdateTransDetRecord(transDetail), PrintTransDetailUpdateError(transDetail))}";
    }

    private string BuildSqlQuery(List<TransDetail> transDetails)
    {
      var transDetailUpdates = transDetails.Select(t => { return SqlIfElseUpdateTransDet(t); });
      var ifTableExistsUpdateElseRollback = SqlIfElse(SqlTableExistsCondition("TRANS_DET"), $"{string.Join("\n", transDetailUpdates)}\n\nCOMMIT TRANSACTION", PrintTableDoesNotExistRollback("TRANS_DET"));
      return $"{SqlPrint("--- SCRIPT EXECUTION COMMENCED ---")}\n\n{SqlUse("CODETABLES")}{SqlBeginTransaction()}{ifTableExistsUpdateElseRollback}\n{SqlPrint("--- SCRIPT EXECUTION COMPLETE ---")}";
    }
  }
}

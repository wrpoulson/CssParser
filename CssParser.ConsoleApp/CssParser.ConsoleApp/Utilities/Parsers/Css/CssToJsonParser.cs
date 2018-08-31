using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using CssParser.ConsoleApp.Models.ClaimForm;
using System.Linq;

namespace CssParser.ConsoleApp.Utilities.Parsers.Css
{
  public class CssToJsonParser
  {
    const string OUTPUT_PATH = @"Content\OutputJSON\";

    public void ParseSourceCssFiles(string sourceFilesPath, bool includeModifiedCss)
    {
      if (!Directory.Exists(sourceFilesPath)) throw new DirectoryNotFoundException();
      FileInfo[] files = new DirectoryInfo(sourceFilesPath).GetFiles("*.css");
      files.ToList().ForEach(file => ParseSourceCssFile(file.FullName, file.Name, includeModifiedCss));
    }

    public void ParseSourceCssFile(string sourceFilePath, string currentFileName, bool includeModifiedCss)
    {
      var transDetailParseResponse = ParseTransDetailsFromFile(sourceFilePath);
      SaveParsedTransDetails(transDetailParseResponse.ParsedTransDetails, currentFileName, currentFileName.Contains("Modified"));
      SaveParseResults(transDetailParseResponse.ParseResult, currentFileName, currentFileName.Contains("Modified"));
      if (includeModifiedCss)
      {
        string modifiedFileName = currentFileName.Replace(".css", ".Modified.css");
        ParseSourceCssFile(sourceFilePath.Replace(currentFileName, $"{@"Modified\"}{modifiedFileName}"), modifiedFileName, false);
      }
    }

    private TransDetailParseResponse ParseTransDetailsFromFile(string sourceFilePath)
    {
      int jsonRecordId = 0;
      int errorRecordId = 0;
      int potentialRecordCount = 0;
      var data = File.ReadAllLines(sourceFilePath);
      string transDetailName = GetTransDetailName(sourceFilePath);
      List<TransDetailError> errors = new List<TransDetailError>();
      TransDetailParseResponse transDetailParseResponse = new TransDetailParseResponse { ParsedTransDetails = new List<TransDetail>() };

      for (var i = 0; i < data.Length; i++)
      {
        TransDetail transDetail = null;
        TransDetailError transDetailError = null;
        int fileLineNumber = i + 1;

        if (data[i]?.Trim().StartsWith("#txt") ?? false)
        {
          potentialRecordCount++;
          string fieldName = data[i].Replace("#txt", "")?.Trim();
          transDetailError = new TransDetailError();
          transDetailError.FieldName = fieldName;

          if (data[i]?.Trim().EndsWith(",") ?? false)
          {
            transDetailError.IsLineEndingWithComma = true;
          }

          if ((data[i + 1]?.Trim().StartsWith("{") ?? false) && fieldName != null)
          {
            if (data[i + 1].Contains("height: 20px"))
            {
              string cssProperties = data[i + 1].Replace("{", "").Replace("}", "").ToLower();
              transDetail = BuildTransDetail(fieldName, cssProperties);
            }
            else
            {
              transDetailError.IsErrorDueToHeight20Px = true;
            }
          }
          else
          {
            transDetailError.IsErrorDueToBrace = true;
          }
        }

        if(transDetail != null)
        {
          transDetail.FileLineNumber = fileLineNumber;
          transDetail.Name = transDetailName;
        }

        if (IsTransDetailValid(transDetail))
        {
          transDetail.JsonRecordId = ++jsonRecordId;
          transDetailParseResponse.ParsedTransDetails.Add(transDetail);
        }
        else if (transDetailError != null)
        {
          transDetailError.FileLineNumber = fileLineNumber;
          transDetailError.IsXpCssLeftNull = transDetail?.Xp_Css_Left == null;
          transDetailError.IsXpCssTopNull = transDetail?.Xp_Css_Top == null;
          transDetailError.IsXpCssWidthNull = transDetail?.Xp_Css_Width == null;
          transDetailError.ErrorRecordId = ++errorRecordId;
          transDetailError.InvalidTransDetail = transDetail;
          errors.Add(transDetailError);
        }
      }

      transDetailParseResponse.ParseResult = BuildParseResult(potentialRecordCount, transDetailParseResponse.ParsedTransDetails, errors);

      return transDetailParseResponse;
    }

    private void SaveParsedTransDetails(List<TransDetail> parsedTransDetails, string outputFileName, bool isModifiedCss)
    {
      string outputPath = isModifiedCss ? $"{OUTPUT_PATH}Modified\\" : OUTPUT_PATH;
      JsonSerializer serializer = new JsonSerializer();
      if (!Directory.Exists(outputPath))
      {
        Directory.CreateDirectory(outputPath);
      }
      File.WriteAllText($"{outputPath}{outputFileName}.TransDetails.json", JsonConvert.SerializeObject(parsedTransDetails, Formatting.Indented));
    }

    private void SaveParseResults(TransDetailParseResult transDetailParseResult, string outputFileName, bool isModifiedCss)
    {
      string outputPath = isModifiedCss ? $"{OUTPUT_PATH}Modified\\" : OUTPUT_PATH;
      JsonSerializer serializer = new JsonSerializer();
      if (!Directory.Exists(outputPath))
      {
        Directory.CreateDirectory(outputPath);
      }
      File.WriteAllText($"{outputPath}{outputFileName}.ParseResults.json", JsonConvert.SerializeObject(transDetailParseResult, Formatting.Indented));
    }

    private TransDetail BuildTransDetail(string fieldName, string cssProperties)
    {
      TransDetail transDetail = new TransDetail { FieldName = fieldName };
      var parts = cssProperties.Split(';');

      foreach (var part in parts)
      {
        int number = 0;
        if (part.Contains("width"))
        {
          if (int.TryParse(part.Replace("width:", "").Replace("px", "").Trim(), out number))
          {
            transDetail.Xp_Css_Width = number;
            number = 0;
          }
        }
        else if (part.Contains("left"))
        {
          if (int.TryParse(part.Replace("left:", "").Replace("px", "").Trim(), out number))
          {
            transDetail.Xp_Css_Left = number;
            number = 0;
          }
        }
        else if (part.Contains("top"))
        {
          if (int.TryParse(part.Replace("top:", "").Replace("px", "").Trim(), out number))
          {
            transDetail.Xp_Css_Top = number;
            number = 0;
          }
        }
      }
      return transDetail;
    }

    private TransDetailParseResult BuildParseResult(int potentialRecordCount, List<TransDetail> parsedTransDetails, List<TransDetailError> errors)
    {
      return new TransDetailParseResult
      {
        PotentialLinesIdentified = potentialRecordCount,
        SavedTransDetails = parsedTransDetails.Count,
        ErrorDetails = new TransDetailParseErrorInfo { Errors = errors }
      };
    }

    private bool IsTransDetailValid(TransDetail transDetail)
    {
      if (transDetail == null) return false;
      if (transDetail.Xp_Css_Left == null || transDetail.Xp_Css_Top == null || transDetail.Xp_Css_Width == null) return false;
      return true;
    }

    private string GetTransDetailName(string sourceFilePath)
    {
      if (sourceFilePath?.Contains("ClaimFormUB") ?? false) return "UB";
      if (sourceFilePath?.Contains("ClaimFormHCFATertiary") ?? false) return "HCFA";
      return null;
    }
  }
}

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using CssParser.ConsoleApp.Models;
using System.Linq;

namespace CssParser.ConsoleApp.Utilities
{
  public class ClaimFormCssParser
  {
    const string OUTPUT_PATH = @"Content\OutputJSON\";

    public void ParseSourceCssFiles(string sourceFilesPath)
    {
      if (!Directory.Exists(sourceFilesPath)) throw new DirectoryNotFoundException();
      FileInfo[] files = new DirectoryInfo(sourceFilesPath).GetFiles("*.css");
      files.ToList().ForEach(file => ParseSourceCssFile(file.FullName, file.Name));
    }

    public void ParseSourceCssFile(string sourceFilePath, string currentFileName)
    {
      var transDetailParseResponse = ParseTransDetailsFromFile(sourceFilePath);
      var fileName = $"{currentFileName}_{DateTime.Now.ToString("yyyy-dd-M--HH-mm-ss")}";
      SaveParsedTransDetails(transDetailParseResponse.ParsedTransDetails, fileName);
      SaveParsedResults(transDetailParseResponse.ParseResult, fileName);
    }

    private TransDetailParseResponse ParseTransDetailsFromFile(string sourceFilePath)
    {
      int jsonRecordId = 0;
      int potentialRecordCount = 0;
      int potentialLinesEndedWithCommaCount = 0;
      var data = File.ReadAllLines(sourceFilePath);
      List<TransDetailError> savedErrors = new List<TransDetailError>();
      List<TransDetailError> unsavedErrors = new List<TransDetailError>();
      TransDetailParseResponse transDetailParseResponse = new TransDetailParseResponse { ParsedTransDetails = new List<TransDetail>() };
      
      for (var i = 0; i < data.Length; i++)
      {
        TransDetail transDetail = null;
        TransDetailError transDetailError = new TransDetailError();

        if (data[i]?.Trim().StartsWith("#txt") ?? false)
        {
          potentialRecordCount++;
          string fieldName = data[i].Replace("#txt", "")?.Trim();
          transDetailError.FieldName = fieldName;
          transDetailError.FileLineNumber = i + 1;

          if (data[i]?.Trim().EndsWith(",") ?? false)
          {
            potentialLinesEndedWithCommaCount++;
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

        if (transDetail != null)
        {
          transDetail.FileLineNumber = i + 1;
          transDetail.JsonRecordId = ++jsonRecordId;
          transDetailParseResponse.ParsedTransDetails.Add(transDetail);
        }

        if ((transDetail != null && !IsTransDetailValid(transDetail)) || transDetailError.IsErrorDueToHeight20Px || transDetailError.IsLineEndingWithComma || transDetailError.IsErrorDueToBrace)
        {
          transDetailError.IsXpCssLeftNull = transDetail?.Xp_Css_Left == null;
          transDetailError.IsXpCssTopNull = transDetail?.Xp_Css_Top == null;
          transDetailError.IsXpCssWidthNull = transDetail?.Xp_Css_Width == null;

          if (transDetail != null)
          {
            transDetailError.IsTransDetailSaved = true;
            savedErrors.Add(transDetailError);
          }
          else
          {
            unsavedErrors.Add(transDetailError);
          }
        }
      }

      transDetailParseResponse.ParseResult = BuildParseResult(potentialRecordCount, potentialLinesEndedWithCommaCount, transDetailParseResponse.ParsedTransDetails, savedErrors, unsavedErrors);

      return transDetailParseResponse;
    }

    private void SaveParsedTransDetails(List<TransDetail> parsedTransDetails, string outputFileName)
    {
      JsonSerializer serializer = new JsonSerializer();
      if (!Directory.Exists(OUTPUT_PATH))
      {
        Directory.CreateDirectory(OUTPUT_PATH);
      }
      File.WriteAllText($"{OUTPUT_PATH}{outputFileName}.TransDetails.json", JsonConvert.SerializeObject(parsedTransDetails, Formatting.Indented));
    }

    private void SaveParsedResults(TransDetailParseResult transDetailParseResult, string outputFileName)
    {
      JsonSerializer serializer = new JsonSerializer();
      if (!Directory.Exists(OUTPUT_PATH))
      {
        Directory.CreateDirectory(OUTPUT_PATH);
      }
      File.WriteAllText($"{OUTPUT_PATH}{outputFileName}.ParseResults.json", JsonConvert.SerializeObject(transDetailParseResult, Formatting.Indented));
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

    private TransDetailParseResult BuildParseResult(int potentialRecordCount, int potentialLinesEndedWithCommaCount, List<TransDetail> parsedTransDetails, List<TransDetailError> savedErrors, List<TransDetailError> unsavedErrors)
    {
      return new TransDetailParseResult
      {
        PotentialLinesIdentified = potentialRecordCount,
        PotentialLinesEndedWithComma = potentialLinesEndedWithCommaCount,
        SavedTransDetails = parsedTransDetails.Count,
        XpCssLeftNull = parsedTransDetails.Where(m => m.Xp_Css_Left == null).Count(),
        XpCssTopNull = parsedTransDetails.Where(m => m.Xp_Css_Top == null).Count(),
        XpCssWidthNull = parsedTransDetails.Where(m => m.Xp_Css_Width == null).Count(),
        SavedErrors = savedErrors,
        UnsavedErrors = unsavedErrors
      };
    }

    private bool IsTransDetailValid(TransDetail transDetail)
    {
      if (transDetail == null) return false;
      if (transDetail.Xp_Css_Left == null || transDetail.Xp_Css_Top == null || transDetail.Xp_Css_Width == null) return false;
      return true;
    }
  }
}

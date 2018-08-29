using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using CssParser.ConsoleApp.Models;

namespace CssParser.ConsoleApp.Utilities
{
  public class ClaimFormCSSParser
  {

    public void ParseSourceCSSFile()
    {
      //DirectoryInfo directoryInfo = new DirectoryInfo(@"Content\SourceCSS");
      var currentFileName = "ClaimFormUB.css";
      var parsedTransDets = ParseTransDetRecordsFromFile(currentFileName);
      var fileName = $"{currentFileName}_{DateTime.Now.ToString("yyyy-dd-M--HH-mm-ss")}";
      SaveParsedTransDet(parsedTransDets, @"Content\OutputJSON\", fileName);
    }

    public List<TransDetRecord> ParseTransDetRecordsFromFile(string fileName)
    {
      var data = File.ReadAllLines(@"Content\SourceCSS\" + fileName);
      var parsedTransDets = new List<TransDetRecord>();
      var jsonRecordId = 0;

      for (var i = 0; i < data.Length; i++)
      {
        TransDetRecord transDetRecord = null;
        if (data[i]?.Trim().StartsWith("#txt") ?? false)
        {
          string fieldName = data[i].Replace("#txt", "")?.Trim();

          if ((data[i + 1]?.Trim().StartsWith("{") ?? false) && data[i + 1].Contains("height: 20px") && fieldName != null)
          {
            var cssProperties = data[i + 1].Replace("{", "").Replace("}", "").ToLower();
            transDetRecord = BuildRecord(fieldName, cssProperties);
          }
        }
        if(transDetRecord != null)
        {
          transDetRecord.JsonRecordId = ++jsonRecordId;
          transDetRecord.FileReadLineId = i;
          parsedTransDets.Add(transDetRecord);
        }
      }
      return parsedTransDets;
    }

    public void SaveParsedTransDet(List<TransDetRecord> parsedTransDets, string outputPath, string outputFileName)
    {
      JsonSerializer serializer = new JsonSerializer();
      if (!Directory.Exists(outputPath))
      {
        Directory.CreateDirectory(outputPath);
      }
      File.WriteAllText(outputPath + outputFileName + ".json", JsonConvert.SerializeObject(parsedTransDets));
    }

    public TransDetRecord BuildRecord(string fieldName, string cssProperties)
    {
      TransDetRecord transDetRecord = new TransDetRecord { FieldName = fieldName  };
      var parts = cssProperties.Split(';');

      foreach (var part in parts)
      {
        int number = 0;
        if (part.Contains("width"))
        {
          if (int.TryParse(part.Replace("width:", "").Replace("px", "").Trim(), out number))
          {
            transDetRecord.Xp_Css_Width = number;
            number = 0;
          }
        }
        else if (part.Contains("left"))
        {
          if (int.TryParse(part.Replace("left:", "").Replace("px", "").Trim(), out number))
          {
            transDetRecord.Xp_Css_Left = number;
            number = 0;
          }
        }
        else if (part.Contains("top"))
        {
          if (int.TryParse(part.Replace("top:", "").Replace("px", "").Trim(), out number))
          {
            transDetRecord.Xp_Css_Top = number;
            number = 0;
          }
        }
      }
      return transDetRecord;
    }
  }
}
